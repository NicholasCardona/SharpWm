using SharpWM.Common;

namespace SharpWM.Tests;
public class ContainerTests
{
    [Fact]
    public void AddChild_SetsParent()
    {
        var monitor = new MonitorContainer { DeviceName = "DISPLAY1" };
        var workspace = new WorkspaceContainer { Name = "1" };
        monitor.AddChild(workspace);
        
        Assert.Equal(monitor, workspace.Parent);
        Assert.Contains(workspace, monitor.Children);
    }

    [Fact]
    public void RemoveChild_ClearsParent()
    {
        var monitor = new MonitorContainer { DeviceName = "DISPLAY1" };
        var workspace = new WorkspaceContainer { Name = "1" };
        monitor.AddChild(workspace);

        var result = monitor.RemoveChild(workspace);

        Assert.True(result);
        Assert.Null(workspace.Parent);
        Assert.DoesNotContain(workspace, monitor.Children);
    }

    [Fact]
    public void RemmoveChild_ReturnsFalse_WhenNotChild()
    {
        var monitor = new MonitorContainer { DeviceName = "DISPLAY1" };
        var workspace = new WorkspaceContainer { Name = "1" };
        var result = monitor.RemoveChild(workspace);
        Assert.False(result);
    }

    [Fact]
    public void Descendants_ReturnAllNodes_BFS()
    {
        var root = new RootContainer();
        var monitor = new MonitorContainer { DeviceName = "DISPLAY1" };
        var workspace = new WorkspaceContainer { Name = "1" };
        var window = new WindowContainer { Handle = 1, Title = "Notepad" };

        root.AddChild(monitor);
        monitor.AddChild(workspace);
        workspace.AddChild(window);

        var descendants = root.Descendants().ToList();

        Assert.Equal(3, descendants.Count);
        Assert.Contains(monitor, descendants);
        Assert.Contains(workspace, descendants);
        Assert.Contains(window, descendants);
    }

    [Fact]
    public void Descendants_EmptyContainer_ReturnsEmpty()
    {
        var root = new RootContainer();
        Assert.Empty(root.Descendants());
    }
}