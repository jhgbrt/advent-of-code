namespace AdventOfCode.Year2021.Day03;

public class AoC202103 : AoCBase
{
    static string[] input = Read.SampleLines(typeof(AoC202103));
    static ImmutableArray<int> numbers = input.Select(s => Convert.ToInt32(s, 2)).ToImmutableArray();

    public override object Part1() => GetBitmasks(numbers).Multiply();

    public override object Part2()
    {
        //while (numbers.Skip(1).Any())
        //{
        //    numbers = numbers.
        //}
        return -1;
    }
    private static (int max, int min) GetBitmasks(IEnumerable<int> numbers) => (
                    from p in
                        from n in numbers
                        from shift in Range(0, input[0].Length)
                        let bit = (n >> shift) & 1
                        group bit by (bit, shift) into g
                        let bit = g.Key.bit
                        let shift = g.Key.shift
                        let count = g.Count()
                        group (count, bit) by shift into g
                        select (shift: g.Key, max: g.MaxBy(x => x.count).bit, min: g.MinBy(x => x.count).bit)
                    select (max: p.max << p.shift, min: p.min << p.shift)
                ).Aggregate((max: 0, min: 0), (n, i) => (n.max | i.max, n.min | i.min));
}

static class Extension
{
    public static int Multiply(this (int x, int y) p) => p.x * p.y;
}
