namespace Net.Code.AdventOfCode.Toolkit.Core;

internal class PuzzleLockedException : AoCException 
{
    public PuzzleLockedException() : base()
    {
    }

    public PuzzleLockedException(string? message) : base(message)
    {
    }

    public PuzzleLockedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
