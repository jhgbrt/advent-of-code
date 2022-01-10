using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Net.Code.AdventOfCode.Tool.Core;

public static class Extensions
{
    public static string ToHumanReadableString(this TimeSpan t) => t switch
    {
        { TotalSeconds: < 1 } => $"{t.Milliseconds} ms",
        { TotalMinutes: < 1 } => $@"{t:s\.f} s",
        { TotalHours: < 1 } => $@"{t:mm\:ss} m",
        _ => t.ToString()
    };
    public static string GetDisplayName<T>(this T e) where T : Enum
    {
        return typeof(T).GetMember(e.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.GetName() ?? e.ToString();
    }
}
