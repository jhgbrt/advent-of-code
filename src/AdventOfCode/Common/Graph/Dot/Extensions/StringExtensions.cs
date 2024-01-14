namespace Net.Code.Graph.Dot.Extensions;

internal static class StringExtensions
{
    internal static string FormatGraphvizEscapedCharacters(this string value)
    {
        return value.Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n");
    }
}