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

// ── 4. CommandDispatcher ─────────────────────────────────────────────────────

var dispatcher = new CommandDispatcher(state);

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
                var cmd = CommandParser.Parse(commandStr);
                var result = dispatcher.Dispatch(cmd);
                if (!result.Success)
                    Console.Error.WriteLine($"[SharpWM] Comando fallito: {result.Error}");
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
Console.WriteLine("[SharpWM] In esecuzione. Premi Ctrl+C per uscire.");

// ── 6. Message loop ──────────────────────────────────────────────────────────

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