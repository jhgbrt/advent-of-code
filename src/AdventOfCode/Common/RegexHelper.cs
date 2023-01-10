namespace AdventOfCode;

internal static class RegexHelper
{

    public static T As<T>(this Regex regex, string s, object? unmatchedValues = null, IFormatProvider? provider = null) where T : struct
    {
        var match = regex.Match(s);
        if (!match.Success) throw new InvalidOperationException($"input '{s}' does not match regex '{regex}'");


        var constructor = typeof(T).GetConstructors().Single();

        var matchedValues = from p in constructor.GetParameters()
                            join m in match.Groups.OfType<Group>() on p.Name equals m.Name
                            select (Key: m.Name, Value: MyConvert.ChangeType(m.Value, p.ParameterType, provider ?? CultureInfo.InvariantCulture));

        var expando = from property in (unmatchedValues ??= new { }).GetType().GetProperties()
                      select (Key: property.Name, Value: property.GetValue(unmatchedValues));

        var keyvalues = (
            from kv in matchedValues.Concat(expando).Select(kv => (kv.Key, kv.Value))
            select (Name: kv.Key, kv.Value)
            ).ToArray();

        var values = (
            from p in constructor.GetParameters()
            join kv in keyvalues on p.Name equals kv.Name
            select kv.Value
            ).ToArray();

        if (constructor.GetParameters().Length != values.Length)
        {
            var unmatchedConstructorArguments = from p in constructor.GetParameters()
                                                join m in match.Groups.OfType<Group>() on p.Name equals m.Name into g
                                                where !g.Any()
                                                select p.Name;
            var unmatchedRegexCaptureGroups = from m in match.Groups.OfType<Group>()
                                              join p in constructor.GetParameters() on m.Name equals p.Name into g
                                              where !g.Any()
                                              select m.Name;
            throw new ArgumentException($"Could not match constructor arguments when converting input to {typeof(T)}. \r\n" +
                $"The following arguments where not matched: {string.Join(",", unmatchedConstructorArguments)}\r\n" +
                $"The following capture groups from the regex where not matched: {string.Join(",", unmatchedRegexCaptureGroups)}");

        }

        return (T)constructor.Invoke(values);

    }



    public static int GetInt32(this Match m, string name) => int.Parse(m.Groups[name].Value);

}
internal static class MyConvert
{
    internal static object ChangeType(string value, Type type, IFormatProvider provider)
    {
        if (type.IsArray)
        {
            var elementType = type.GetElementType();
            if (elementType is null) throw new ArgumentNullException("array does not have element type?!??");
            return ConvertToArray(value, elementType, provider);
        }
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            var elementType = type.GetGenericArguments().Single();
            if (elementType is null) throw new ArgumentNullException("enumerable does not have element type?!??");
            return ConvertToArray(value, elementType, provider);
        }

        return Convert.ChangeType(value, type, provider);
    }

    static object ConvertToArray(string value, Type elementType, IFormatProvider provider)
    {
        string delimiter = ",";
        if (provider is CsvLineFormatInfo c)
        {
            delimiter = c.Delimiter;
        }
        var values = value.Split(delimiter, StringSplitOptions.TrimEntries);
        var tmp = values.Select(v => Convert.ChangeType(v, elementType, provider)).ToArray();
        var result = Array.CreateInstance(elementType, values.Length);
        Array.Copy(tmp, result, values.Length);
        return result;
    }
}
record CsvLineFormatInfo(string Delimiter = ",") : IFormatProvider, ICustomFormatter
{
    public object? GetFormat(Type? type) => this;

    public string Format(string? format, object? arg, IFormatProvider? formatProvider)
    {
        var array = arg as Array ?? throw new NullReferenceException(nameof(arg));
        return string.Join(format ?? Delimiter, array.OfType<object>());
    }
}

