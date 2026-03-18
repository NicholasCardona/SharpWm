using SharpWM.Common;
using SharpWM.Config;

namespace SharpWM.Tests;

public class KeybindingParserTests
{
    [Theory]
    [InlineData("alt+1",       KeyModifiers.Alt,                    0x31)]
    [InlineData("alt+shift+q", KeyModifiers.Alt | KeyModifiers.Shift, 'Q')]
    [InlineData("alt+h",       KeyModifiers.Alt,                    'H')]
    [InlineData("ctrl+f4",     KeyModifiers.Ctrl,                   0x73)]
    [InlineData("alt+left",    KeyModifiers.Alt,                    0x25)]
    [InlineData("alt+escape",  KeyModifiers.Alt,                    0x1B)]
    public void Parse_ReturnsCorrectModifiersAndKey(
        string binding, KeyModifiers expectedMod, uint expectedVk)
    {
        var (mods, vk) = KeybindingParser.Parse(binding);

        Assert.Equal(expectedMod, mods);
        Assert.Equal(expectedVk, vk);
    }

    [Fact]
    public void Parse_Throws_ForUnrecognizedKey()
    {
        Assert.Throws<ConfigException>(() =>
            KeybindingParser.Parse("alt+nonexistent"));
    }

    [Fact]
    public void Parse_IsCaseInsensitive()
    {
        var (mods1, vk1) = KeybindingParser.Parse("ALT+H");
        var (mods2, vk2) = KeybindingParser.Parse("alt+h");

        Assert.Equal(mods1, mods2);
        Assert.Equal(vk1, vk2);
    }
}