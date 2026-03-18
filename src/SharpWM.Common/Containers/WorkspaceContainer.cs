namespace SharpWM.Common;

public sealed class WorkspaceContainer : Container
{
    public string Name { get; init; } = string.Empty;
    public bool IsActive { get; set; }
    public Direction TilingDirection { get; set; } = Direction.Horizontal;

    public WorkspaceContainer()
    {
        Type = ContainerType.Workspace;
    }
}