using SharpWM.Common;

namespace SharpWM.Config;

/// <summary>
/// Converts a string like "alt+shift+1" in (KeyModifiers, VirtualKey).
/// </summary>
public static class KeybindingParser
{
    public static (KeyModifiers Modifiers, uint VirtualKey) Parse(string binding)
    {
        var parts = binding.ToLowerInvariant()
            .Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        var modifiers = KeyModifiers.None;
        uint vk = 0;

        foreach (var part in parts)
        {
            switch (part)
            {
                case "alt":   modifiers |= KeyModifiers.Alt;   break;
                case "ctrl":
                case "control": modifiers |= KeyModifiers.Ctrl; break;
                case "shift": modifiers |= KeyModifiers.Shift; break;
                case "win":   modifiers |= KeyModifiers.Win;   break;
                default:
                    vk = ParseKey(part);
                    break;
            }
        }

        if (vk == 0)
            throw new ConfigException($"Keybinding non valido — tasto non riconosciuto: '{binding}'");

        return (modifiers, vk);
    }

    private static uint ParseKey(string key) => key switch
    {
        // Lettere
        var k when k.Length == 1 && char.IsLetter(k[0]) =>
            (uint)char.ToUpperInvariant(k[0]),

        // Numeri riga superiore
        var k when k.Length == 1 && char.IsDigit(k[0]) =>
            (uint)('0' + (k[0] - '0')),

        // Tasti funzione
        "f1"  => 0x70, "f2"  => 0x71, "f3"  => 0x72, "f4"  => 0x73,
        "f5"  => 0x74, "f6"  => 0x75, "f7"  => 0x76, "f8"  => 0x77,
        "f9"  => 0x78, "f10" => 0x79, "f11" => 0x7A, "f12" => 0x7B,

        // Navigazione
        "left"      => 0x25,
        "up"        => 0x26,
        "right"     => 0x27,
        "down"      => 0x28,
        "enter"     => 0x0D,
        "escape"    => 0x1B,
        "space"     => 0x20,
        "tab"       => 0x09,
        "backspace" => 0x08,
        "delete"    => 0x2E,
        "home"      => 0x24,
        "end"       => 0x23,
        "pageup"    => 0x21,
        "pagedown"  => 0x22,

        _ => 0
    };
}