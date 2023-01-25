namespace AdventOfCode.Year2019.Day08;

public class AoC201908
{
    internal static string[] input = Read.InputLines();
    IEnumerable<int[]> layers = input.SelectMany(c => c).Select(c => c - '0').Chunk(25*6);
    public object Part1() => (
        from layer in layers
        select (layer, count: layer.Count(x => x == 0))
        ).MinBy(x => x.count).layer
        .Where(i => i is 1 or 2)
        .Aggregate((ones: 0, twos: 0), (p, i) => i == 1 ? (p.ones + 1, p.twos) : (p.ones, p.twos + 1)).Product();

    public object Part2()
    {
        var result = layers.Aggregate(
            Repeat(2, 25 * 6), 
            (accumulation, layer) => accumulation.Zip(layer).Select(x => x.First == 2 ? x.Second : x.First)
            );

        var sb = new StringBuilder();
        foreach (var (line, y) in result.Chunk(25).Select((line, y) => (line, y)))
        {
            foreach (var (i, x) in line.Select((c,x) => (c,x)))
            {
                sb.Append(i == 1 ? '#' : '.');
            }
            sb.AppendLine();
        }
        return sb.ToString().DecodePixels(AsciiFontSize._4x6);
    }
}

public static class Ex
{
    public static int Product(this (int x, int y) p) => p.x * p.y;
}
