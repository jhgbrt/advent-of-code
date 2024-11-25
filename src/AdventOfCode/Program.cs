using Net.Code.AdventOfCode.Toolkit;

FormatTime(TimeSpan.FromMinutes(5));

var sw = Stopwatch.StartNew();

await AoC.RunAsync(args);

double bytes = GC.GetTotalAllocatedBytes();
string bytesAllocated = FormatBytes(bytes);

var ts = sw.Elapsed;
string duration = FormatTime(ts);

var separator = "+" + new string(Repeat('-', 78).ToArray()) + "+";
Console.WriteLine(separator);
Console.WriteLine("| Total time      | " + duration.PadLeft(58) + " |");
Console.WriteLine(separator);
Console.WriteLine("| Bytes allocated | " + bytesAllocated.PadLeft(58) + " |");
Console.WriteLine(separator);

static string FormatBytes(double bytes)
{
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    int n = 0;
    while (bytes >= 1024 && n < sizes.Length - 1)
    {
        n++;
        bytes /= 1024;
    }
    var bytesAllocated = $"{bytes:0.0000} {sizes[n]}";
    return bytesAllocated;
}

static string FormatTime(TimeSpan ts)
{
    return ts switch
    {
        { TotalHours: > 1 } => $@"{ts:hh\:mm\:ss}",
        { TotalMinutes: > 1 } => $@"{ts:mm\:ss}",
        { TotalSeconds: > 10 } => $"{ts.TotalSeconds} s",
        { TotalSeconds: > 1 } => $@"{ts:ss\.fff} s",
        { TotalMilliseconds: > 10 } => $"{ts.TotalMilliseconds} ms",
        { TotalMilliseconds: > 1 } => $"{ts.TotalMicroseconds} μs",
        _ => $"{ts.TotalNanoseconds} ns"
    };
}