using System.Runtime.Versioning;
using SharpWM.Common;
using Windows.Win32;
using Windows.Win32.UI.Input.KeyboardAndMouse;

namespace SharpWM.Platform;

/// <summary>
/// Registra hotkey globali con RegisterHotKey e le espone come eventi.
/// </summary>
[SupportedOSPlatform("windows6.0.6000")]
public sealed class KeyboardHook : IDisposable
{
    private readonly Dictionary<int, Action> _bindings = [];
    private int _nextId = 1;
    private bool _disposed;

    public event Action<string>? OnError;

    /// <summary>
    /// Registra una combinazione di tasti. Restituisce false se già occupata dal sistema.
    /// </summary>
    public bool Register(KeyModifiers modifiers, uint virtualKey, Action callback)
    {
        int id = _nextId++;

        // HOT_KEY_MODIFIERS è internal in CsWin32 — cast esplicito tramite uint
        var csModifiers = (HOT_KEY_MODIFIERS)(uint)modifiers;

        if (!PInvoke.RegisterHotKey(default, id, csModifiers, virtualKey))
        {
            OnError?.Invoke($"Impossibile registrare hotkey id={id} vk={virtualKey}");
            return false;
        }

        _bindings[id] = callback;
        return true;
    }

    /// <summary>
    /// Rimuove tutte le hotkey registrate.
    /// </summary>
    public void UnregisterAll()
    {
        foreach (var id in _bindings.Keys)
            PInvoke.UnregisterHotKey(default, id);

        _bindings.Clear();
    }

    /// <summary>
    /// Chiamato dal message loop quando arriva WM_HOTKEY.
    /// </summary>
    public void HandleHotKey(int id)
    {
        if (_bindings.TryGetValue(id, out var action))
            action();
    }

    public void Dispose()
    {
        if (_disposed) return;
        UnregisterAll();
        _disposed = true;
    }
}