namespace AdventOfCode.Client.Logic;

public static class TimespanExtensions
{
    public static string ToHumanReadableString(this TimeSpan t)
    {
        return t switch
        {
            { TotalSeconds: < 1 } => $"{t.Milliseconds} ms",
            { TotalMinutes: < 1 } => $@"{t:s\.f} s",
            { TotalHours: < 1 } => $@"{t:h:mm}",
            _ => t.ToString()
        };
    }
}
