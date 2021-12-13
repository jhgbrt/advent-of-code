using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace AdventOfCode.Client.Logic;

public static class TypeExtensions
{
    public static string GetDisplayName<T>(this T e) where T : Enum
    {
        return typeof(T)
        .GetMember(e.ToString())
        .First()
        .GetCustomAttribute<DisplayAttribute>()
        ?.GetName() ?? e.ToString();
    }
        
}