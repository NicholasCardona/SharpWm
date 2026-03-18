using SharpWM.Common;

namespace SharpWM.Tests;

public class WmStateTests
{
    private static WmState BuildBasicState()
    {
        var state = new WmState();
        var monitor = new MonitorContainer{ DeviceName = "DISPLAY1", Bounds = new Rect(0, 0, 1920, 1080) };
        var workspace = new WorkspaceContainer { Name = "1", IsActive = true };
        var window = new WindowContainer { Handle = 100, Title = "Notepad", ProcessName = "notepad" };

        monitor.AddChild(workspace);
        workspace.AddChild(window);
        state.AddMonitor(monitor);

        return state;
    }

#region monitor
    [Fact]
    public void AddMonitor_AppearsInAllMonitors()
    {
        var state = new WmState();
        var monitor = new MonitorContainer { DeviceName = "DISPLAY1" };
        state.AddMonitor(monitor);
        Assert.Single(state.AllMonitors);
        Assert.Equal("DISPLAY1", state.AllMonitors.First().DeviceName);
    }
    [Fact]
    public void GetMonitor_ReturnsCorrectMonitor()
    {
        var state = BuildBasicState();
        var monitor = state.GetMonitor("DISPLAY1");
        Assert.NotNull(monitor);
        Assert.Equal("DISPLAY1", monitor.DeviceName);
    }

    [Fact]
    public void GetMonitor_ReturnsNull_WhenNotFound()
    {
        var state = new WmState();
        Assert.Null(state.GetMonitor("DISPLAY99"));
    }
#endregion

#region workspace
    [Fact]
    public void GetWorkspace_ReturnsCorrectWorkspace()
    {
        var state = BuildBasicState();
        var ws = state.GetWorkspace("1");

        Assert.NotNull(ws);
        Assert.Equal("1", ws.Name);
    }

    [Fact]
    public void GetActiveWorkspace_ReturnsActiveOne()
    {
        var state = BuildBasicState();
        var monitor = state.GetMonitor("DISPLAY1");
        Assert.NotNull(monitor);
        var active = state.GetActiveWorkspace(monitor);
        Assert.NotNull(active);
        Assert.True(active.IsActive);
    }
#endregion

#region window
        [Fact]
    public void FindWindow_ReturnsCorrectWindow()
    {
        var state = BuildBasicState();
 
        var window = state.FindWindow(100);
 
        Assert.NotNull(window);
        Assert.Equal("Notepad", window.Title);
    }
 
    [Fact]
    public void FindWindow_ReturnsNull_WhenNotFound()
    {
        var state = BuildBasicState();
 
        Assert.Null(state.FindWindow(999));
    }
 
    [Fact]
    public void AllWindows_ReturnsAllWindowContainers()
    {
        var state = BuildBasicState();
        var ws = state.GetWorkspace("1")!;
        ws.AddChild(new WindowContainer { Handle = 200, Title = "VS Code" });
 
        var windows = state.AllWindows.ToList();
 
        Assert.Equal(2, windows.Count);
    }
#endregion

#region Focus
[Fact]
    public void SetFocus_UpdatesFocusedWindow()
    {
        var state = BuildBasicState();
        var window = state.FindWindow(100)!;
 
        state.SetFocus(window);
 
        Assert.Equal(window, state.FocusedWindow);
    }
 
    [Fact]
    public void SetFocus_Null_ClearsFocus()
    {
        var state = BuildBasicState();
        state.SetFocus(state.FindWindow(100));
 
        state.SetFocus(null);
 
        Assert.Null(state.FocusedWindow);
    }
#endregion

#region navigation
    [Fact]
    public void WorkspaceOf_ReturnsCorrectWorkspace()
    {
        var state = BuildBasicState();
        var window = state.FindWindow(100)!;
        var ws = state.WorkspaceOf(window);
        Assert.NotNull(ws);
        Assert.Equal("1", ws.Name);
    }

    [Fact]
    public void MonitorOf_ReturnsCorrectMonitor()
    {
        var state = BuildBasicState();
        var window = state.FindWindow(100)!;
        var monitor = state.MonitorOf(window);
        Assert.NotNull(monitor);
        Assert.Equal("DISPLAY1", monitor.DeviceName);
    }
 
    [Fact]
    public void WorkspaceOf_ReturnsNull_ForOrphanWindow()
    {
        var state = new WmState();
        var window = new WindowContainer { Handle = 999 };
        Assert.Null(state.WorkspaceOf(window));
    }
#endregion
}