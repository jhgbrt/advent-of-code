namespace AdventOfCode.Year2015.Day16;

public partial class AoC201516
{
    static string[] lines = Read.InputLines();
    static readonly Regex regex = SueRegex();

    static ImmutableList<Sue> sues = (
        from line in lines
        let data = regex.As<SueData>(line)
        let number = data.number
        let properties = (
        from propertylist in data.properties.Split(',', StringSplitOptions.TrimEntries)
        let kv = propertylist.Split(':', StringSplitOptions.TrimEntries)
        select (key: kv[0], value: int.Parse(kv[1]))
    ).ToDictionary(x => x.key, x => x.value)
        select new Sue(number, properties)
            ).ToImmutableList();

    static ImmutableDictionary<string, int> list = new Dictionary<string, int>()
    {
        ["children"] = 3,
        ["cats"] = 7,
        ["samoyeds"] = 2,
        ["pomeranians"] = 3,
        ["akitas"] = 0,
        ["vizslas"] = 0,
        ["goldfish"] = 5,
        ["trees"] = 3,
        ["cars"] = 2,
        ["perfumes"] = 1,
    }.ToImmutableDictionary();


    public object Part1() => (
            from sue in sues
            where sue.properties.All(p => p.Value == list[p.Key])
            select sue
            ).Single().number;

    public object Part2() => (
            from sue in sues
            where sue.HasEqual("children", list)
            && sue.HasMore("cats", list)
            && sue.HasEqual("samoyeds", list)
            && sue.HasLess("pomeranians", list)
            && sue.HasEqual("akitas", list)
            && sue.HasEqual("vizslas", list)
            && sue.HasLess("goldfish", list)
            && sue.HasMore("trees", list)
            && sue.HasEqual("cars", list)
            && sue.HasEqual("perfumes", list)
            select sue
            ).Single().number;
    [GeneratedRegex("Sue (?<number>\\d+): (?<properties>.*)")]
    private static partial Regex SueRegex();
}
record struct SueData(int number, string properties);
record Sue(int number, IReadOnlyDictionary<string, int> properties)
{
    public bool HasEqual(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] == list[name];
    public bool HasMore(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] > list[name];
    public bool HasLess(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] < list[name];
}
