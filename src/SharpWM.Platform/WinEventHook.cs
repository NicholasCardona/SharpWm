using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace SharpWM.Platform;

/// <summary>
/// Tipo di evento Windows intercettato da WinEventHook.
/// </summary>
public enum WinEvent
{
    WindowCreated,
    WindowDestroyed,
    WindowFocused
}

/// <summary>
/// Dati dell'evento: handle della finestra e tipo.
/// </summary>
public readonly record struct WinEventArgs(nint Handle, WinEvent Event);

/// <summary>
/// Intercetta eventi di sistema tramite SetWinEventHook.
/// Emette un evento .NET per ogni finestra creata, distrutta o focalizzata.
/// </summary>
[SupportedOSPlatform("windows5.0")]
public sealed class WinEventHook : IDisposable
{
    private delegate void WinEventProc(
        nint hWinEventHook, uint eventType,
        nint hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime);

    [DllImport("user32.dll")]
    private static extern nint SetWinEventHook(
        uint eventMin, uint eventMax,
        nint hmodWinEventProc, WinEventProc lpfnWinEventProc,
        uint idProcess, uint idThread, uint dwFlags);

    [DllImport("user32.dll")]
    private static extern bool UnhookWinEvent(nint hWinEventHook);

    // Costanti Win32
    private const uint EVENT_OBJECT_CREATE      = 0x8000;
    private const uint EVENT_OBJECT_DESTROY     = 0x8001;
    private const uint EVENT_SYSTEM_FOREGROUND  = 0x0003;
    private const uint WINEVENT_OUTOFCONTEXT    = 0x0000;
    private const int  OBJID_WINDOW             = 0;

    public event Action<WinEventArgs>? OnWindowEvent;

    private readonly List<nint> _hooks = [];
    private readonly WinEventProc _proc; // Teniamo il delegate vivo per evitare GC
    private bool _disposed;

    public WinEventHook()
    {
        // Il delegate deve restare vivo per tutta la vita dell'hook
        _proc = HandleWinEvent;
    }

    /// <summary>
    /// Installa gli hook di sistema. Da chiamare sul thread con message loop.
    /// </summary>
    public void Install()
    {
        _hooks.Add(SetWinEventHook(EVENT_OBJECT_CREATE,     EVENT_OBJECT_CREATE,     IntPtr.Zero, _proc, 0, 0, WINEVENT_OUTOFCONTEXT));
        _hooks.Add(SetWinEventHook(EVENT_OBJECT_DESTROY,    EVENT_OBJECT_DESTROY,    IntPtr.Zero, _proc, 0, 0, WINEVENT_OUTOFCONTEXT));
        _hooks.Add(SetWinEventHook(EVENT_SYSTEM_FOREGROUND, EVENT_SYSTEM_FOREGROUND, IntPtr.Zero, _proc, 0, 0, WINEVENT_OUTOFCONTEXT));
    }

    private void HandleWinEvent(
        nint hWinEventHook, uint eventType,
        nint hwnd, int idObject, int idChild,
        uint dwEventThread, uint dwmsEventTime)
    {
        // Filtriamo solo eventi sulle finestre reali (non su oggetti interni)
        if (idObject != OBJID_WINDOW || hwnd == IntPtr.Zero)
            return;

        var winEvent = eventType switch
        {
            EVENT_OBJECT_CREATE     => WinEvent.WindowCreated,
            EVENT_OBJECT_DESTROY    => WinEvent.WindowDestroyed,
            EVENT_SYSTEM_FOREGROUND => WinEvent.WindowFocused,
            _ => (WinEvent?)null
        };

        if (winEvent is null) return;

        OnWindowEvent?.Invoke(new WinEventArgs(hwnd, winEvent.Value));
    }

    public void Dispose()
    {
        if (_disposed) return;
        foreach (var hook in _hooks)
            if (hook != IntPtr.Zero)
                UnhookWinEvent(hook);
        _hooks.Clear();
        _disposed = true;
    }
}