using Net.Code.AdventOfCode.Toolkit;

var sw = Stopwatch.StartNew();

await AoC.RunAsync(args);

var bytesAllocated = GC.GetTotalAllocatedBytes().FormatBytes();

var ts = sw.Elapsed;
string duration = ts.FormatTime();

var separator = "+" + new string(Repeat('-', 78).ToArray()) + "+";
Console.WriteLine(separator);
Console.WriteLine("| Total time      | " + duration.PadLeft(58) + " |");
Console.WriteLine(separator);
Console.WriteLine("| Bytes allocated | " + bytesAllocated.PadLeft(58) + " |");
Console.WriteLine(separator);

static class Helpers
{
    internal static string FormatBytes(this long b)
    {
        double bytes = b;
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        int n = 0;
        while (bytes >= 1024 && n < sizes.Length - 1)
        {
            n++;
            bytes /= 1024;
        }
        return $"{bytes:0.0000} {sizes[n]}";
    }

    internal static string FormatTime(this TimeSpan timespan) => timespan switch
    {
        { TotalHours: > 1 } ts => $@"{ts:hh\:mm\:ss}",
        { TotalMinutes: > 1 } ts => $@"{ts:mm\:ss}",
        { TotalSeconds: > 10 } ts => $"{ts.TotalSeconds} s",
        { TotalSeconds: > 1 } ts => $@"{ts:ss\.fff} s",
        { TotalMilliseconds: > 10 } ts => $"{ts.TotalMilliseconds} ms",
        { TotalMilliseconds: > 1 } ts => $"{ts.TotalMicroseconds} μs",
        TimeSpan ts => $"{ts.TotalNanoseconds} ns"
    };
}
