using SharpWM.Common;

namespace SharpWM.Core;

/// <summary>Moves the focus to the selected direction.</summary>
public record FocusCommand(Direction Direction) : ICommand;

/// <summary>moves the focused window in the specified direction.</summary>
public record MoveWindowCommand(Direction Direction) : ICommand;
 
/// <summary>Activate workspace with the specified name.</summary>
public record FocusWorkspaceCommand(string WorkspaceName) : ICommand;
 
/// <summary>Moves the focused windows on another workspace.</summary>
public record MoveToWorkspaceCommand(string WorkspaceName) : ICommand;
 
/// <summary>Changes split direction of the active workspace.</summary>
public record SetTilingDirectionCommand(Direction Direction) : ICommand;
 
/// <summary>Closes the focused window.</summary>
public record CloseWindowCommand : ICommand;