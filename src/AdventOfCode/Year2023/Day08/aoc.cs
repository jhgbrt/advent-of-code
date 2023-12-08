namespace AdventOfCode.Year2023.Day08;
public class AoC202308
{
    static string[] input = Read.InputLines();
    static string steps = input[0];
    static ImmutableDictionary<string, Item> dictionary = input[2..].Select(s => Regexes.MyRegex().As<Item>(s)).ToImmutableDictionary(x => x.name);
    public object Part1() => CalculateSteps(dictionary, "AAA", "ZZZ");
    public object Part2() => dictionary.Keys.Where(x => x[2] == 'A').Select(n => CalculateSteps(dictionary, n, "Z")).LeastCommonMultiplier();

    private int CalculateSteps(IReadOnlyDictionary<string, Item> nodes, string start, string end)
    {
        int i = 0;
        string node = start;
        while (!node.EndsWith(end))
        {
            var step = steps[i % steps.Length];
            node = step switch
            {
                'R' => nodes[node].right,
                'L' => nodes[node].left
            };
            i++;
        }
        return i;
    }
}

readonly record struct Item(string name, string left, string right);

static partial class Regexes
{
    [GeneratedRegex(@"^(?<name>.{3}) = \((?<left>.{3}), (?<right>.{3})\)$")]
    public static partial Regex MyRegex();
}