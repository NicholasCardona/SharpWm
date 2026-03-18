using SharpWM.Common;

namespace SharpWM.Core;

/// <summary>
/// Recieves commands and routes them to the correct manager,
/// modifiing the WmState consequently.
/// </summary>
public sealed class CommandDispatcher
{
    private readonly WmState _state;

    public CommandDispatcher(WmState state)
    {
        _state = state;
    }

    public CommandResult Dispatch(ICommand command) => command switch
    {
        FocusWorkspaceCommand   c => HandleFocusWorkspace(c),
        MoveToWorkspaceCommand  c => HandleMoveToWorkspace(c),
        SetTilingDirectionCommand c => HandleSetTilingDirection(c),
        CloseWindowCommand        _ => HandleCloseWindow(),
        FocusCommand              c => HandleFocus(c),
        MoveWindowCommand         c => HandleMoveWindow(c),
        _ => CommandResult.Fail($"Comando non gestito: {command.GetType().Name}")
    };

    // ── Focus workspace ─────────────────────────────────────────

    private CommandResult HandleFocusWorkspace(FocusWorkspaceCommand cmd)
    {
        var target = _state.GetWorkspace(cmd.WorkspaceName);
        if (target is null)
            return CommandResult.Fail($"Workspace '{cmd.WorkspaceName}' non trovato");

        // Disattiva il workspace corrente sullo stesso monitor
        var monitor = _state.AllMonitors
            .FirstOrDefault(m => m.Workspaces.Contains(target));

        if (monitor is null)
            return CommandResult.Fail("Monitor non trovato per il workspace");

        foreach (var ws in monitor.Workspaces)
            ws.IsActive = false;

        target.IsActive = true;

        // Focus alla prima finestra del workspace (se presente)
        var firstWindow = target.Descendants()
            .OfType<WindowContainer>()
            .FirstOrDefault();

        _state.SetFocus(firstWindow);
        return CommandResult.Ok();
    }

    // ── Move to workspace ────────────────────────────────────────

    private CommandResult HandleMoveToWorkspace(MoveToWorkspaceCommand cmd)
    {
        if (_state.FocusedWindow is null)
            return CommandResult.NoOp();

        var target = _state.GetWorkspace(cmd.WorkspaceName);
        if (target is null)
            return CommandResult.Fail($"Workspace '{cmd.WorkspaceName}' non trovato");

        var window = _state.FocusedWindow;
        window.Parent?.RemoveChild(window);
        target.AddChild(window);

        return CommandResult.Ok();
    }

    // ── Tiling direction ─────────────────────────────────────────

    private CommandResult HandleSetTilingDirection(SetTilingDirectionCommand cmd)
    {
        if (_state.FocusedWindow is null)
            return CommandResult.NoOp();

        var ws = _state.WorkspaceOf(_state.FocusedWindow);
        if (ws is null)
            return CommandResult.NoOp();

        ws.TilingDirection = cmd.Direction;
        return CommandResult.Ok();
    }

    // ── Close window ─────────────────────────────────────────────

    private CommandResult HandleCloseWindow()
    {
        if (_state.FocusedWindow is null)
            return CommandResult.NoOp();

        var window = _state.FocusedWindow;
        window.Parent?.RemoveChild(window);
        _state.SetFocus(null);
        return CommandResult.Ok();
    }

    // ── Focus direction ──────────────────────────────────────────

    private CommandResult HandleFocus(FocusCommand cmd)
    {
        if (_state.FocusedWindow is null)
            return CommandResult.NoOp();

        var ws = _state.WorkspaceOf(_state.FocusedWindow);
        if (ws is null)
            return CommandResult.NoOp();

        var windows = ws.Descendants()
            .OfType<WindowContainer>()
            .ToList();

        if (windows.Count < 2)
            return CommandResult.NoOp();

        int idx = windows.IndexOf(_state.FocusedWindow);
        int next = cmd.Direction is Direction.Horizontal
            ? (idx + 1) % windows.Count
            : (idx - 1 + windows.Count) % windows.Count;

        _state.SetFocus(windows[next]);
        return CommandResult.Ok();
    }

    // ── Move window ──────────────────────────────────────────────

    private CommandResult HandleMoveWindow(MoveWindowCommand cmd)
    {
        if (_state.FocusedWindow is null)
            return CommandResult.NoOp();

        var ws = _state.WorkspaceOf(_state.FocusedWindow);
        if (ws is null)
            return CommandResult.NoOp();

        var windows = ws.Descendants()
            .OfType<WindowContainer>()
            .ToList();

        if (windows.Count < 2)
            return CommandResult.NoOp();

        int idx = windows.IndexOf(_state.FocusedWindow);
        int swapIdx = cmd.Direction is Direction.Horizontal
            ? (idx + 1) % windows.Count
            : (idx - 1 + windows.Count) % windows.Count;

        // Scambia le posizioni nell'albero
        var a = windows[idx];
        var b = windows[swapIdx];
        var parentA = a.Parent!;
        var parentB = b.Parent!;
        int posA = parentA.Children.IndexOf(a);
        int posB = parentB.Children.IndexOf(b);

        parentA.Children[posA] = b;
        parentB.Children[posB] = a;
        b.Parent = parentA;
        a.Parent = parentB;

        return CommandResult.Ok();
    }
}