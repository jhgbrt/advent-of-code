using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Net.Code.AdventOfCode.Tool.Core;

public static class Extensions
{
    public static string GetDisplayName<T>(this T e) where T : Enum
    {
        return typeof(T).GetMember(e.ToString()).First().GetCustomAttribute<DisplayAttribute>()?.GetName() ?? e.ToString();
    }
}
