namespace SharpWM.Config;

 
/// <summary>
/// Configuration root, Corresponds to the file config.yaml
/// </summary>
public sealed class WmConfig
{
    public List<WorkspaceConfig> Workspaces { get; init; } = [];
    public List<KeybindingConfig> Keybindings { get; init; } = [];
    public GapsConfig Gaps { get; init; } = new();
}