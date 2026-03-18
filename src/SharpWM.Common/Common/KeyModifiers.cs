namespace SharpWM.Common;

/// <summary>
/// Keybindings modifiers
/// Public conversion of HOT_KEY_MODIFIERS from CsWin32
/// </summary>
[Flags]
public enum KeyModifiers : uint
{
    None    = 0x0000,
    Alt     = 0x0001,
    Ctrl    = 0x0002,
    Shift   = 0x0004,
    Win     = 0x0008,
}