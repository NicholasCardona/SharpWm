using SharpWM.Common;
using SharpWM.Core;

namespace SharpWM.Tests;

public class CommandDispatcherTests
{
    private static (WmState state, CommandDispatcher dispatcher) BuildSetup(
        string[] workspaceNames,
        int windowCount = 2)
    {
        var state = new WmState();
        var monitor = new MonitorContainer
        {
            DeviceName = "DISPLAY1",
            Bounds = new Rect(0, 0, 1920, 1080)
        };

        for (int i = 0; i < workspaceNames.Length; i++)
        {
            var ws = new WorkspaceContainer
            {
                Name = workspaceNames[i],
                IsActive = i == 0
            };

            if (i == 0)
            {
                for (int w = 0; w < windowCount; w++)
                    ws.AddChild(new WindowContainer
                    {
                        Handle = 100 + w,
                        Title  = $"Window {w}"
                    });
            }

            monitor.AddChild(ws);
        }

        state.AddMonitor(monitor);
        var firstWindow = state.AllWindows.FirstOrDefault();
        state.SetFocus(firstWindow);

        return (state, new CommandDispatcher(state));
    }

    // ── FocusWorkspace ───────────────────────────────────────────

    [Fact]
    public void FocusWorkspace_ActivatesTargetWorkspace()
    {
        var (state, dispatcher) = BuildSetup(["1", "2"]);

        var result = dispatcher.Dispatch(new FocusWorkspaceCommand("2"));

        Assert.True(result.Success);
        Assert.True(state.GetWorkspace("2")!.IsActive);
        Assert.False(state.GetWorkspace("1")!.IsActive);
    }

    [Fact]
    public void FocusWorkspace_ReturnsFailure_WhenNotFound()
    {
        var (_, dispatcher) = BuildSetup(["1"]);

        var result = dispatcher.Dispatch(new FocusWorkspaceCommand("99"));

        Assert.False(result.Success);
        Assert.NotNull(result.Error);
    }

    // ── MoveToWorkspace ──────────────────────────────────────────

    [Fact]
    public void MoveToWorkspace_MovesWindowToTargetWorkspace()
    {
        var (state, dispatcher) = BuildSetup(["1", "2"]);
        var window = state.FocusedWindow!;

        var result = dispatcher.Dispatch(new MoveToWorkspaceCommand("2"));

        Assert.True(result.Success);
        Assert.Contains(window, state.GetWorkspace("2")!.Children);
        Assert.DoesNotContain(window, state.GetWorkspace("1")!.Children);
    }

    [Fact]
    public void MoveToWorkspace_NoOp_WhenNoFocusedWindow()
    {
        var (state, dispatcher) = BuildSetup(["1", "2"]);
        state.SetFocus(null);

        var result = dispatcher.Dispatch(new MoveToWorkspaceCommand("2"));

        Assert.True(result.Success);
        Assert.Empty(state.GetWorkspace("2")!.Children);
    }

    // ── SetTilingDirection ───────────────────────────────────────

    [Fact]
    public void SetTilingDirection_ChangesWorkspaceDirection()
    {
        var (state, dispatcher) = BuildSetup(["1"]);

        dispatcher.Dispatch(new SetTilingDirectionCommand(Direction.Vertical));

        Assert.Equal(Direction.Vertical, state.GetWorkspace("1")!.TilingDirection);
    }

    // ── CloseWindow ──────────────────────────────────────────────

    [Fact]
    public void CloseWindow_RemovesWindowAndClearsFocus()
    {
        var (state, dispatcher) = BuildSetup(["1"], windowCount: 1);
        var window = state.FocusedWindow!;

        var result = dispatcher.Dispatch(new CloseWindowCommand());

        Assert.True(result.Success);
        Assert.Null(state.FocusedWindow);
        Assert.DoesNotContain(window, state.AllWindows);
    }

    // ── FocusCommand ─────────────────────────────────────────────

    [Fact]
    public void Focus_CyclesWindowsInWorkspace()
    {
        var (state, dispatcher) = BuildSetup(["1"], windowCount: 3);
        var first = state.FocusedWindow!;

        dispatcher.Dispatch(new FocusCommand(Direction.Horizontal));

        Assert.NotEqual(first, state.FocusedWindow);
    }

    // ── MoveWindowCommand ────────────────────────────────────────

    [Fact]
    public void MoveWindow_SwapsWindowPositions()
    {
        var (state, dispatcher) = BuildSetup(["1"], windowCount: 2);
        var ws = state.GetWorkspace("1")!;
        var first  = (WindowContainer)ws.Children[0];
        var second = (WindowContainer)ws.Children[1];
        state.SetFocus(first);

        dispatcher.Dispatch(new MoveWindowCommand(Direction.Horizontal));

        Assert.Equal(second, ws.Children[0]);
        Assert.Equal(first,  ws.Children[1]);
    }

    // ── Unknown command ──────────────────────────────────────────

    [Fact]
    public void Dispatch_ReturnsFailure_ForUnknownCommand()
    {
        var (_, dispatcher) = BuildSetup(["1"]);

        var result = dispatcher.Dispatch(new UnknownCommand());

        Assert.False(result.Success);
    }

    private record UnknownCommand : ICommand;
}