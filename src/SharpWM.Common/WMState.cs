namespace SharpWM.Common;

/// <summary>
/// WM Central State. 
/// Contains the container's tree referred to the focused window.
/// </summary>
public sealed class WmState
{
    public RootContainer Root { get; } = new();
    public WindowContainer? FocusedWindow { get; private set; }


    public void AddMonitor(MonitorContainer monitor) => Root.AddChild(monitor);
    public MonitorContainer? GetMonitor(string deviceName) => Root.Monitors.FirstOrDefault(m => m.DeviceName == deviceName);
    public IEnumerable<MonitorContainer> AllMonitors => Root.Monitors;


    public WorkspaceContainer? GetWorkspace(string name) => Root.Descendants().OfType<WorkspaceContainer>().FirstOrDefault(w => w.Name == name);
    public WorkspaceContainer? GetActiveWorkspace(MonitorContainer monitor) => monitor.ActiveWorkspace;


    public WindowContainer? FindWindow(nint handle) => Root.Descendants().OfType<WindowContainer>().FirstOrDefault(w => w.Handle == handle);
    public IEnumerable<WindowContainer> AllWindows => Root.Descendants().OfType<WindowContainer>();
    public void SetFocus(WindowContainer? window)
    {
        FocusedWindow = window;
    }

    // ── Utility ─────────────────────────────────────────────────
    /// <summary>
    /// Returns the workspace that contains the specified window.
    /// </summary>
    public WorkspaceContainer? WorkspaceOf(WindowContainer window)
    {
        Container? current = window.Parent;
        while (current is not null)
        {
            if (current is WorkspaceContainer ws) return ws;
            current = current.Parent;
        }
        return null;
    }
    /// <summary>
    /// Returns the montitor that contains the specified window.
    /// </summary>
    public MonitorContainer? MonitorOf(WindowContainer window)
    {
        Container? current = window.Parent;
        while (current is not null)
        {
            if (current is MonitorContainer m) return m;
            current = current.Parent;
        }
        return null;
    }
}