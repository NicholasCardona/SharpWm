using SharpWM.Common;

namespace SharpWM.Core;

/// <summary>
/// Converts a string in a command (es. "focus --workspace 1") in a ICommand.
/// </summary>
public static class CommandParser
{
    public static ICommand Parse(string raw)
    {
        var parts = raw.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            throw new ArgumentException($"Empty command: '{raw}'");

        return parts[0].ToLowerInvariant() switch
        {
            "focus"  => ParseFocus(parts),
            "move"   => ParseMove(parts),
            "close"  => new CloseWindowCommand(),
            "tiling-direction" => ParseTilingDirection(parts),
            _ => throw new ArgumentException($"Command failed: '{parts[0]}'")
        };
    }

    private static ICommand ParseFocus(string[] parts)
    {
        var (flag, value) = GetFlagValue(parts);
        return flag switch
        {
            "--workspace" => new FocusWorkspaceCommand(value),
            "--direction" => new FocusCommand(ParseDirection(value)),
            _ => throw new ArgumentException($"Invalid Flag for focus: '{flag}'")
        };
    }

    private static ICommand ParseMove(string[] parts)
    {
        var (flag, value) = GetFlagValue(parts);
        return flag switch
        {
            "--workspace" => new MoveToWorkspaceCommand(value),
            "--direction" => new MoveWindowCommand(ParseDirection(value)),
            _ => throw new ArgumentException($"Invalid Flag for move: '{flag}'")
        };
    }

    private static ICommand ParseTilingDirection(string[] parts)
    {
        var (_, value) = GetFlagValue(parts);
        return new SetTilingDirectionCommand(ParseDirection(value));
    }

    private static (string Flag, string Value) GetFlagValue(string[] parts)
    {
        if (parts.Length < 3)
            throw new ArgumentException($"Incomplete Command: '{string.Join(' ', parts)}'");
        return (parts[1], parts[2]);
    }

    private static Direction ParseDirection(string value) =>
        value.ToLowerInvariant() switch
        {
            "left"  or "horizontal" => Direction.Horizontal,
            "right" or "vertical"   => Direction.Vertical,
            _ => throw new ArgumentException($"Invalid Direction: '{value}'")
        };
}