using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace SharpWM.Config;

public static class ConfigLoader
{
    private static readonly IDeserializer Deserializer =
        new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

    /// <summary>
    /// Load config from specified path.
    /// Throws <see cref="ConfigException"/> file not existing or not right-formatted.
    /// </summary>
    public static WmConfig Load(string path)
    {
        if (!File.Exists(path))
            throw new ConfigException($"File di configurazione non trovato: {path}");

        try
        {
            var yaml = File.ReadAllText(path);
            return Deserializer.Deserialize<WmConfig>(yaml) ?? new WmConfig();
        }
        catch (Exception ex) when (ex is not ConfigException)
        {
            throw new ConfigException($"Errore nel parsing della configurazione: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Load config from default path:
    /// %USERPROFILE%\.glzr\sharpwm\config.yaml
    /// </summary>
    public static WmConfig LoadDefault()
    {
        var path = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".sharpwm", "config.yaml");

        if (!File.Exists(path))
        {
            WriteDefaultConfig(path);
        }

        return Load(path);
    }

    private static void WriteDefaultConfig(string path)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        File.WriteAllText(path, DefaultConfig);
    }

    private const string DefaultConfig = """
        workspaces:
          - name: "1"
          - name: "2"
          - name: "3"
          - name: "4"
          - name: "5"

        gaps:
          inner: 8
          outer: 8

        keybindings:
          - binding: "alt+1"
            command: "focus --workspace 1"
          - binding: "alt+2"
            command: "focus --workspace 2"
          - binding: "alt+3"
            command: "focus --workspace 3"
          - binding: "alt+4"
            command: "focus --workspace 4"
          - binding: "alt+5"
            command: "focus --workspace 5"
          - binding: "alt+shift+1"
            command: "move --workspace 1"
          - binding: "alt+shift+2"
            command: "move --workspace 2"
          - binding: "alt+shift+q"
            command: "close"
          - binding: "alt+h"
            command: "focus --direction left"
          - binding: "alt+l"
            command: "focus --direction right"
          - binding: "alt+shift+h"
            command: "move --direction left"
          - binding: "alt+shift+l"
            command: "move --direction right"
        """;
}