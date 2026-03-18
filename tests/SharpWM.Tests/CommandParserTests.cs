using SharpWM.Common;
using SharpWM.Core;

namespace SharpWM.Tests;

public class CommandParserTests
{
    [Fact]
    public void Parse_FocusWorkspace() =>
        Assert.Equal(new FocusWorkspaceCommand("1"), CommandParser.Parse("focus --workspace 1"));

    [Fact]
    public void Parse_FocusDirection() =>
        Assert.Equal(new FocusCommand(Direction.Horizontal), CommandParser.Parse("focus --direction left"));

    [Fact]
    public void Parse_MoveWorkspace() =>
        Assert.Equal(new MoveToWorkspaceCommand("2"), CommandParser.Parse("move --workspace 2"));

    [Fact]
    public void Parse_MoveDirection() =>
        Assert.Equal(new MoveWindowCommand(Direction.Vertical), CommandParser.Parse("move --direction right"));

    [Fact]
    public void Parse_Close() =>
        Assert.IsType<CloseWindowCommand>(CommandParser.Parse("close"));

    [Fact]
    public void Parse_Throws_ForUnknownCommand() =>
        Assert.Throws<ArgumentException>(() => CommandParser.Parse("unknown --foo bar"));

    [Fact]
    public void Parse_Throws_ForEmptyString() =>
        Assert.Throws<ArgumentException>(() => CommandParser.Parse(""));
}