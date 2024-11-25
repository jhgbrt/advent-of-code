namespace Net.Code.AdventOfCode.Toolkit.Core;

internal class AlreadyCompletedException : AoCException
{
    public AlreadyCompletedException() : base()
    {
    }

    public AlreadyCompletedException(string? message) : base(message)
    {
    }

    public AlreadyCompletedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}