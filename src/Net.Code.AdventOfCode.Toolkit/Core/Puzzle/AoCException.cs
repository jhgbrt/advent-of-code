namespace Net.Code.AdventOfCode.Toolkit.Core;

class AoCException:Exception{
    public AoCException() : base()
    {
    }

    public AoCException(string? message) : base(message)
    {
    }

    public AoCException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
