namespace Net.Code.Graph.Dot;

public record DotIdentifier(string Value, bool IsHtml = false)
{
    public static implicit operator DotIdentifier(string value) => new DotIdentifier(value);
}