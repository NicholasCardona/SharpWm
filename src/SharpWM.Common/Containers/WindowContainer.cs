namespace SharpWM.Common;

public sealed class WindowContainer : Container
{
    public nint Handle { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ProcessName { get; set; } = string.Empty;
    public bool IsFloating { get; set; }
    public Rect LastFloatingBounds { get; set; }

    public WindowContainer()
    {
        Type = ContainerType.Window;
    }
}