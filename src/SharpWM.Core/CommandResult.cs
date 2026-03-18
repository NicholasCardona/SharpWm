namespace SharpWM.Core;

///<summary>
/// Result of a comman execution 
///</summary>
public readonly record struct CommandResult(bool Success, string? Error = null)
{
    public static CommandResult Ok()                    => new(true);
    public static CommandResult Fail(string reason)     => new(false, reason);
    public static CommandResult NoOp()                  => new(true);
}