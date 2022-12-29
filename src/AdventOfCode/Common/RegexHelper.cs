namespace AdventOfCode;

internal static class RegexHelper
{
    public static T As<T>(this Regex regex, string s, IFormatProvider? provider = null) where T: struct
    {
        var match = regex.Match(s);
        if (!match.Success) throw new InvalidOperationException($"input '{s}' does not match regex '{regex}'");

        var constructor = typeof(T).GetConstructors().Single();
        
        var j = from p in constructor.GetParameters()
                join m in match.Groups.OfType<Group>() on p.Name equals m.Name
                select Convert.ChangeType(m.Value, p.ParameterType, provider ?? CultureInfo.InvariantCulture);

        return (T)constructor.Invoke(j.ToArray());

    }

    public static int GetInt32(this Match m, string name) => int.Parse(m.Groups[name].Value);

}

