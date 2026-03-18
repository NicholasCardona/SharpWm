namespace SharpWM.Config;
 
public sealed class KeybindingConfig
{
    /// <summary>Es: "alt+1", "alt+shift+q"</summary>
    public string Binding { get; init; } = string.Empty;
 
    /// <summary>Es: "focus --workspace 1", "close"</summary>
    public string Command { get; init; } = string.Empty;
}