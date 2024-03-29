var lines = File.ReadAllLines("input.txt");
var regex = AoCRegex.SueRegex();
var sues = (
    from line in lines
    let data = regex.As<SueData>(line)
    let number = data.number
    let properties = (
        from propertylist in data.properties.Split(',', StringSplitOptions.TrimEntries) let kv = propertylist.Split(':', StringSplitOptions.TrimEntries) select (key: kv[0], value: int.Parse(kv[1]))).ToDictionary(x => x.key, x => x.value)
    select new Sue(number, properties)).ToImmutableList();
var list = new Dictionary<string, int>()
{ ["children"] = 3, ["cats"] = 7, ["samoyeds"] = 2, ["pomeranians"] = 3, ["akitas"] = 0, ["vizslas"] = 0, ["goldfish"] = 5, ["trees"] = 3, ["cars"] = 2, ["perfumes"] = 1, }.ToImmutableDictionary();
var sw = Stopwatch.StartNew();
var part1 = (
    from sue in sues
    where sue.properties.All(p => p.Value == list[p.Key])
    select sue).Single().number;
var part2 = (
    from sue in sues
    where sue.HasEqual("children", list) && sue.HasMore("cats", list) && sue.HasEqual("samoyeds", list) && sue.HasLess("pomeranians", list) && sue.HasEqual("akitas", list) && sue.HasEqual("vizslas", list) && sue.HasLess("goldfish", list) && sue.HasMore("trees", list) && sue.HasEqual("cars", list) && sue.HasEqual("perfumes", list)
    select sue).Single().number;
Console.WriteLine((part1, part2, sw.Elapsed));
record struct SueData(int number, string properties);
record Sue(int number, IReadOnlyDictionary<string, int> properties)
{
    public bool HasEqual(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] == list[name];
    public bool HasMore(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] > list[name];
    public bool HasLess(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] < list[name];
}

static partial class AoCRegex
{
    [GeneratedRegex("Sue (?<number>\\d+): (?<properties>.*)")]
    public static partial Regex SueRegex();
    public static T As<T>(this Regex regex, string s, IFormatProvider? provider = null)
        where T : struct
    {
        var match = regex.Match(s);
        if (!match.Success)
            throw new InvalidOperationException($"input '{s}' does not match regex '{regex}'");
        var constructor = typeof(T).GetConstructors().Single();
        var j =
            from p in constructor.GetParameters()
            join m in match.Groups.OfType<Group>() on p.Name equals m.Name
            select Convert.ChangeType(m.Value, p.ParameterType, provider ?? CultureInfo.InvariantCulture);
        return (T)constructor.Invoke(j.ToArray());
    }

    public static int GetInt32(this Match m, string name) => int.Parse(m.Groups[name].Value);
}