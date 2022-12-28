namespace AdventOfCode.Year2016.Day06;

public class AoC201606
{
    public static string[] input = Read.InputLines();

    public object Part1() => new Accumulator().Decode(input, 8, false);
    public object Part2() => new Accumulator().Decode(input, 8, true);
}


internal class Accumulator
{
    public string Decode(IEnumerable<string> data, int lineLength, bool ascending = false)
    {
        var query = from line in data
                    from item in line.Select((c, i) => new { c, pos = i })
                    select item;

        var lookup = query.ToLookup(item => item.pos);

        var sb = new StringBuilder();
        for (int i = 0; i < lineLength; i++)
        {
            var g = lookup[i];
            var grpByChar = g.GroupBy(item => item.c);
            var ordered = ascending
                ? grpByChar.OrderBy(x => x.Count())
                : grpByChar.OrderByDescending(x => x.Count());
            var c = ordered.First().First().c;
            sb.Append(c);
        }
        return sb.ToString();
    }
}