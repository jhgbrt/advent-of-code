﻿static class Output
{
    internal static void WriteResult<T>(int part, T value, TimeSpan time, long bytes)
    {
        Console.WriteLine($"Part {part}: {value} ({time.FormatTime()} - {bytes.FormatBytes()})");
    }

    internal static void WriteResult<T1, T2>(T1 part1, T2 part2, TimeSpan time)
    {
        Console.WriteLine($"+".PadRight(39, '-') + "+");
        Console.WriteLine($"| Part 1    | {part1}".PadRight(39) + "|");
        Console.WriteLine($"| Part 2    | {part2}".PadRight(39) + "|");
        Console.WriteLine($"| Time      | {time.FormatTime()}".PadRight(39) + "|");
        Console.WriteLine($"| Allocated | {GC.GetTotalAllocatedBytes().FormatBytes()}".PadRight(39) + "|");
        Console.WriteLine($"+".PadRight(39, '-') + "+");
    }
    static string FormatBytes(this long b)
    {
        double bytes = b;
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        int n = 0;
        while (bytes >= 1024 && n < sizes.Length - 1)
        {
            n++;
            bytes /= 1024;
        }
        return $"{bytes:0.00} {sizes[n]}";
    }
    static string FormatTime(this TimeSpan timespan) => timespan switch
    {
        { TotalHours: > 1 } ts => $@"{ts:hh\:mm\:ss}",
        { TotalMinutes: > 1 } ts => $@"{ts:mm\:ss}",
        { TotalSeconds: > 10 } ts => $"{ts.TotalSeconds} s",
        { TotalSeconds: > 1 } ts => $@"{ts:ss\.fff} s",
        { TotalMilliseconds: > 1 } ts => $"{ts.TotalMilliseconds:0} ms",
        { TotalMicroseconds: > 1 } ts => $"{ts.TotalMicroseconds:0} μs",
        TimeSpan ts => $"{ts.TotalNanoseconds:0} ns"
    };
}