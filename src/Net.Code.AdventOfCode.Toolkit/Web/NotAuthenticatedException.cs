namespace Net.Code.AdventOfCode.Toolkit.Web;

using Net.Code.AdventOfCode.Toolkit.Core;

class NotAuthenticatedException : AoCException
{
    public NotAuthenticatedException() : base()
    {
    }

    public NotAuthenticatedException(string? message) : base(message)
    {
    }

    public NotAuthenticatedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}

