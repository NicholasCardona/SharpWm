namespace SharpWM.Common;

public sealed class MonitorContainer : Container
{
    public string DeviceName { get; init; } = string.Empty;
    public Rect Bounds { get; set; }
    public bool IsPrimary { get; init; }

    public MonitorContainer()
    {
        Type = ContainerType.Monitor;
    }

    public IEnumerable<WorkspaceContainer> Workspaces => Children.OfType<WorkspaceContainer>();
    public WorkspaceContainer? ActiveWorkspace => Workspaces.FirstOrDefault(w => w.IsActive);
}