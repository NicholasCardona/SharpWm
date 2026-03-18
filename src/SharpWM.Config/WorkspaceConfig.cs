namespace SharpWM.Config;
 
public sealed class WorkspaceConfig
{
    public string Name { get; init; } = string.Empty;
    public string? DisplayName { get; init; }
    public bool KeepAlive { get; init; }
}