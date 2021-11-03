using System.Text.RegularExpressions;

var test = false;

var filename = test ? "sample.txt" : "input.txt";

var lines = File.ReadAllLines(filename);
var regex = new Regex(@"Sue (?<number>\d+): (?<properties>.*)");
var sues = (
    from line in lines
        let match = regex.Match(line)
        let number = int.Parse(match.Groups["number"].Value)
        let properties = (
            from propertylist in match.Groups["properties"].Value.Split(',', StringSplitOptions.TrimEntries)
            let kv = propertylist.Split(':', StringSplitOptions.TrimEntries)
            select (key: kv[0], value: int.Parse(kv[1]))
        ).ToDictionary(x => x.key, x => x.value)
        select new Sue(number, properties)
        ).ToList();

var list = new Dictionary<string, int>
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
};


Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() => (
        from sue in sues
        where sue.properties.All(p => p.Value == list[p.Key])
        select sue
        ).Single().number;

int Part2() => (
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

record Sue(int number, IReadOnlyDictionary<string, int> properties)
{
    public bool HasEqual(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] == list[name];
    public bool HasMore(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] > list[name];
    public bool HasLess(string name, IReadOnlyDictionary<string, int> list) => !properties.ContainsKey(name) || properties[name] < list[name];
}
