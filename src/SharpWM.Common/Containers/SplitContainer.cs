namespace SharpWM.Common;

public sealed class SplitContainer : Container
{
    public Direction Direction { get; set; }
    public SplitContainer()
    {
        Type = ContainerType.Split;
    }
}