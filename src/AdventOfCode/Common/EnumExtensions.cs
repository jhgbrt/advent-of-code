using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Common;
public static class EnumExtensions
{
    /// <summary>Whether the given value is defined on its enum type.</summary>
    public static bool IsDefined<T>(this T enumValue) where T : struct, Enum
    {
        return EnumValueCache<T>.DefinedValues.Contains(enumValue);
    }

    private static class EnumValueCache<T> where T : struct, Enum
    {
        public static readonly HashSet<T> DefinedValues = new(Enum.GetValues<T>());
    }
}