using SharpWM.Config;

namespace SharpWM.Tests;

public class ConfigLoaderTests
{
    private static string WriteTempConfig(string yaml)
    {
        var path = Path.GetTempFileName();
        File.WriteAllText(path, yaml);
        return path;
    }

    [Fact]
    public void Load_ParsesWorkspacesCorrectly()
    {
        var path = WriteTempConfig("""
            workspaces:
              - name: "1"
              - name: "2"
                display_name: "Work"
                keep_alive: true
            """);

        var config = ConfigLoader.Load(path);

        Assert.Equal(2, config.Workspaces.Count);
        Assert.Equal("1", config.Workspaces[0].Name);
        Assert.Equal("Work", config.Workspaces[1].DisplayName);
        Assert.True(config.Workspaces[1].KeepAlive);
    }

    [Fact]
    public void Load_ParsesKeybindingsCorrectly()
    {
        var path = WriteTempConfig("""
            keybindings:
              - binding: "alt+1"
                command: "focus --workspace 1"
              - binding: "alt+shift+q"
                command: "close"
            """);

        var config = ConfigLoader.Load(path);

        Assert.Equal(2, config.Keybindings.Count);
        Assert.Equal("alt+1", config.Keybindings[0].Binding);
        Assert.Equal("close", config.Keybindings[1].Command);
    }

    [Fact]
    public void Load_ParsesGapsCorrectly()
    {
        var path = WriteTempConfig("""
            gaps:
              inner: 12
              outer: 16
            """);

        var config = ConfigLoader.Load(path);

        Assert.Equal(12, config.Gaps.Inner);
        Assert.Equal(16, config.Gaps.Outer);
    }

    [Fact]
    public void Load_ReturnsEmptyConfig_ForEmptyFile()
    {
        var path = WriteTempConfig("");

        var config = ConfigLoader.Load(path);

        Assert.NotNull(config);
        Assert.Empty(config.Workspaces);
    }

    [Fact]
    public void Load_Throws_WhenFileNotFound()
    {
        Assert.Throws<ConfigException>(() =>
            ConfigLoader.Load("C:\\nonexistent\\config.yaml"));
    }
}