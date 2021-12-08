namespace AdventOfCode.Year2019.Day08;

public class AoC201908 : AoCBase
{
    internal static string[] input = Read.InputLines(typeof(AoC201908));
    IEnumerable<int[]> layers = input.SelectMany(c => c).Select(c => c - '0').Chunk(25*6);
    public override object Part1() => (
        from layer in layers
        select (layer, count: layer.Count(x => x == 0))
        ).MinBy(x => x.count).layer.Where(i => i is 1 or 2).Aggregate((ones: 0, twos: 0), (p, i) => i == 1 ? (p.ones + 1, p.twos) : (p.ones, p.twos + 1)).Product();

    public override object Part2()
    {
        var result = layers.Aggregate(Repeat(2, 25 * 6), (accumulation, layer) => accumulation.Zip(layer).Select(x => x.First == 2 ? x.Second : x.First));

        var q = from x in result.Chunk(25).Select((line, n) => (line, n))
                let line = x.line
                let n = x.n
                from y in line.Chunk(5).Select((chunk, i) => (chunk, i))
                let chunk = y.chunk
                let i = y.i
                group (chunk, n) by i;

        var resultStringBuilder = new StringBuilder();
        foreach (var group in q)
        {
            var sb = new StringBuilder().AppendLine();
            foreach (var (chunk,n) in group)
            {
                sb.AppendLine(string.Join("", chunk.Take(4).Select(i => i == 1 ? "#" : ".")));
            }
            var letter = sb.ToString() switch
            {
                AsciiLetters.U => 'U',
                AsciiLetters.B => 'B',
                AsciiLetters.F => 'F',
                AsciiLetters.P => 'P',
                _ => '?'
            };
            resultStringBuilder.Append(letter);
        }

        return resultStringBuilder.ToString();
    }
}

public static class Ex
{
    public static int Product(this (int x, int y) p) => p.x * p.y;
}

static class AsciiLetters
{
    public const string U = @"
#..#
#..#
#..#
#..#
#..#
.##.
";
    public const string B = @"
###.
#..#
###.
#..#
#..#
###.
";
    public const string F = @"
####
#...
###.
#...
#...
#...
";
    public const string P = @"
###.
#..#
#..#
###.
#...
#...
";
}