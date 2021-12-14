namespace AdventOfCode.Year2021.Day14;

public class AoC202114 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC202114));

    static string recipe = input[0];

    static ImmutableDictionary<string, (string first, string second)> transformations = (
         from line in input.Skip(2)
         let split = line.Split(" -> ")
         select (src: split[0], dest: split[1])
         ).ToImmutableDictionary(s => s.src, s => (first: s.src.Substring(0, 1) + s.dest, second: s.dest + s.src.Substring(1, 1)));

    static ImmutableDictionary<string, long> start = (
        from i in Range(0, recipe.Length - 1)
        let pair = recipe.Substring(i, 2)
        group pair by pair).ToImmutableDictionary(g => g.Key, g => (long)g.Count());

    public override object Part1() => Compute(10);
    public override object Part2() => Compute(40);

    private static long Compute(int iterations)
    {
        var counts = Range(0, iterations).Aggregate(start, (dictionary, _) => Transform(dictionary, transformations));

        var characters = (from pair in counts
                          let c = pair.Key[0] let count = counts[pair.Key]
                          group count by c into g
                          select (c: g.Key, count: g.Key == recipe.Last() ? g.Sum() + 1 : g.Sum())
                          ).ToImmutableDictionary(c => c.c, c => c.count);

        return characters.Values.Max() - characters.Values.Min();
    }

    private static ImmutableDictionary<string, long> Transform(ImmutableDictionary<string, long> dictionary, IReadOnlyDictionary<string, (string first, string second)> transformations)
        => (from item in dictionary
            let pair = item.Key let count = item.Value
            let p = transformations[pair]
            let first = p.first let second = p.second
            from key in Repeat(first, 1).Concat(Repeat(second, 1))
            group count by key into g
            select (g.Key, Count: g.Sum())).ToImmutableDictionary(g => g.Key, g => g.Count);

}
