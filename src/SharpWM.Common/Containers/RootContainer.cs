namespace SharpWM.Common;

public sealed class RootContainer : Container
{
    public RootContainer()
    {
        Type = ContainerType.Root;
    }

    public IEnumerable<MonitorContainer> Monitors => Children.OfType<MonitorContainer>();
}