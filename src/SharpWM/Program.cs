using System.Runtime.Versioning;
using SharpWM.Common;
using SharpWM.Config;
using SharpWM.Core;
using SharpWM.Platform;

[assembly: SupportedOSPlatform("windows10.0.17763.0")]

// ── 1. Configurazione ────────────────────────────────────────────────────────

WmConfig config;
try
{
    config = ConfigLoader.LoadDefault();
    Console.WriteLine("[SharpWM] Configurazione caricata.");
}
catch (ConfigException ex)
{
    Console.Error.WriteLine($"[SharpWM] Errore configurazione: {ex.Message}");
    return 1;
}

// ── 2. Monitor ───────────────────────────────────────────────────────────────

var monitors = MonitorService.EnumerateMonitors();
Console.WriteLine($"[SharpWM] Monitor rilevati: {monitors.Count}");
foreach (var m in monitors)
    Console.WriteLine($"  {m.DeviceName} {m.Bounds} {(m.IsPrimary ? "(primary)" : "")}");

// ── 3. Stato WM ──────────────────────────────────────────────────────────────

var state = new WmState();
try
{
    WmStartup.Initialize(state, config, monitors);
    Console.WriteLine($"[SharpWM] Workspace inizializzati: {state.AllMonitors.Sum(m => m.Workspaces.Count())}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[SharpWM] Errore inizializzazione: {ex.Message}");
    return 1;
}

// ── 4. TilingEngine e CommandDispatcher ──────────────────────────────────────

var tilingEngine = new TilingEngine(config.Gaps.Inner, config.Gaps.Outer);
var dispatcher   = new CommandDispatcher(state);

// Applica il tiling su tutti i monitor
void ApplyTiling()
{
    foreach (var monitor in state.AllMonitors)
    {
        var ws = monitor.ActiveWorkspace;
        if (ws is null) continue;

        var layout = tilingEngine.Calculate(ws, monitor.Bounds);
        foreach (var (window, rect) in layout)
            WindowService.SetWindowBounds(window.Handle, rect);
    }
}

// ── 5. Keybinding ────────────────────────────────────────────────────────────

using var keyboard = new KeyboardHook();
keyboard.OnError += msg => Console.Error.WriteLine($"[SharpWM] Hotkey: {msg}");

foreach (var kb in config.Keybindings)
{
    try
    {
        var (modifiers, vk) = KeybindingParser.Parse(kb.Binding);
        var commandStr = kb.Command;

        keyboard.Register(modifiers, vk, () =>
        {
            try
            {
                var cmd    = CommandParser.Parse(commandStr);
                var result = dispatcher.Dispatch(cmd);
                if (!result.Success)
                    Console.Error.WriteLine($"[SharpWM] Comando fallito: {result.Error}");
                else
                    ApplyTiling();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"[SharpWM] Errore comando '{commandStr}': {ex.Message}");
            }
        });
    }
    catch (ConfigException ex)
    {
        Console.Error.WriteLine($"[SharpWM] Keybinding ignorato '{kb.Binding}': {ex.Message}");
    }
}

Console.WriteLine($"[SharpWM] {config.Keybindings.Count} keybinding registrati.");

// ── 6. WinEventHook ───────────────────────────────────────────────────────────

using var winEvents = new WinEventHook();

winEvents.OnWindowEvent += args =>
{
    switch (args.Event)
    {
        case WinEvent.WindowCreated:
            // Ignora finestre non gestibili (toolbar, popup, ecc.)
            if (!WindowService.IsManageable(args.Handle)) break;

            var activeWs = state.AllMonitors
                .Select(m => m.ActiveWorkspace)
                .FirstOrDefault(ws => ws is not null);

            if (activeWs is null) break;

            // Evita duplicati
            if (state.FindWindow(args.Handle) is not null) break;

            var title = WindowService.GetWindowTitle(args.Handle);
            activeWs.AddChild(new WindowContainer
            {
                Handle = args.Handle,
                Title  = title
            });

            Console.WriteLine($"[SharpWM] Finestra aggiunta: '{title}' ({args.Handle})");
            ApplyTiling();
            break;

        case WinEvent.WindowDestroyed:
            var existing = state.FindWindow(args.Handle);
            if (existing is null) break;

            existing.Parent?.RemoveChild(existing);
            if (state.FocusedWindow?.Handle == args.Handle)
                state.SetFocus(null);

            Console.WriteLine($"[SharpWM] Finestra rimossa: ({args.Handle})");
            ApplyTiling();
            break;

        case WinEvent.WindowFocused:
            var focused = state.FindWindow(args.Handle);
            if (focused is not null)
                state.SetFocus(focused);
            break;
    }
};

winEvents.Install();
Console.WriteLine("[SharpWM] In esecuzione. Premi Ctrl+C per uscire.");

// ── 7. Message loop ───────────────────────────────────────────────────────────

var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

try
{
    await Task.Delay(Timeout.Infinite, cts.Token);
}
catch (OperationCanceledException) { }

Console.WriteLine("[SharpWM] Uscita.");
return 0;