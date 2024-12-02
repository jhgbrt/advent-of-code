using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Net.Code.AdventOfCode.Toolkit.Infrastructure;

public static class Extensions
{
    public static string GetDisplayName<T>(this T e) where T : Enum
    {
        return typeof(T).GetMember(e.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.GetName() ?? e.ToString();
    }

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
        return $"{bytes:0} {sizes[n]}";
    }

    internal static string FormatTimeSpan(this TimeSpan timespan) => timespan switch
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
