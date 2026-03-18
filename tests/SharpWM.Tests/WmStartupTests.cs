using SharpWM.Common;
using SharpWM.Config;
using SharpWM.Core;

namespace SharpWM.Tests;

public class WmStartupTests
{
    private static MonitorContainer MakeMonitor(string name, bool primary = true) =>
        new() { DeviceName = name, Bounds = new Rect(0, 0, 1920, 1080), IsPrimary = primary };

    [Fact]
    public void Initialize_CreatesWorkspacesOnPrimaryMonitor()
    {
        var state = new WmState();
        var config = new WmConfig
        {
            Workspaces = [new() { Name = "1" }, new() { Name = "2" }, new() { Name = "3" }]
        };

        WmStartup.Initialize(state, config, [MakeMonitor("DISPLAY1")]);

        var monitor = state.GetMonitor("DISPLAY1")!;
        Assert.Equal(3, monitor.Workspaces.Count());
    }

    [Fact]
    public void Initialize_FirstWorkspaceIsActive()
    {
        var state = new WmState();
        var config = new WmConfig
        {
            Workspaces = [new() { Name = "1" }, new() { Name = "2" }]
        };

        WmStartup.Initialize(state, config, [MakeMonitor("DISPLAY1")]);

        Assert.True(state.GetWorkspace("1")!.IsActive);
        Assert.False(state.GetWorkspace("2")!.IsActive);
    }

    [Fact]
    public void Initialize_UsesDefaultWorkspaces_WhenConfigEmpty()
    {
        var state = new WmState();
        var config = new WmConfig();

        WmStartup.Initialize(state, config, [MakeMonitor("DISPLAY1")]);

        var monitor = state.GetMonitor("DISPLAY1")!;
        Assert.Equal(5, monitor.Workspaces.Count());
    }

    [Fact]
    public void Initialize_Throws_WhenNoMonitors()
    {
        var state = new WmState();
        var config = new WmConfig();

        Assert.Throws<InvalidOperationException>(() =>
            WmStartup.Initialize(state, config, []));
    }

    [Fact]
    public void Initialize_MultiMonitor_AllAddedToState()
    {
        var state = new WmState();
        var config = new WmConfig
        {
            Workspaces = [new() { Name = "1" }]
        };
        var monitors = new[]
        {
            MakeMonitor("DISPLAY1", primary: true),
            MakeMonitor("DISPLAY2", primary: false)
        };

        WmStartup.Initialize(state, config, monitors);

        Assert.Equal(2, state.AllMonitors.Count());
    }
}