namespace AdventOfCode.Year2017.Day06;

public class AoC201706
{

    public static string[] input = Read.InputLines();

    public object Part1() => Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()).steps;
    public object Part2() => Memory.Cycles(new byte[] { 10, 3, 15, 10, 5, 15, 5, 15, 9, 2, 5, 8, 5, 2, 3, 6 }.ToImmutableArray()).loopSize;

}

static class Memory
{
    public static (int steps, int loopSize) Cycles(ImmutableArray<byte> input)
    {
        var list = new List<ImmutableArray<byte>>();
        var (steps, max, cycle) = (0, 0, input);

        while (true)
        {
            var m = cycle.FindMax();
            if (m.value > max)
                max = m.value;
            steps++;
            cycle = DoOneCycle(cycle);
            var index = list.FindIndex(x => x.SequenceEqual(cycle));
            if (index >= 0)
            {
                return (steps, list.Count - index);
            }
            list.Add(cycle);
        };
    }

    public static (int index, int value) FindMax(this IEnumerable<byte> input)
        => input.Select((value, index) => (index: index, value: value))
            .Aggregate((index: -1, value: int.MinValue), (x, y) => y.value > x.value ? y : x);

    public static ImmutableArray<byte> DoOneCycle(ImmutableArray<byte> input)
    {
        var (max, length) = (input.FindMax(), input.Length);
        var (quotient, remainder) = (max.value / length, max.value % length);

        var query =
            from t in input.Select((value, i) => (value: value, i: i))
            let i = t.i
            let value = t.value
            let j = (i + length - max.index - 1) % length
            let term1 = i == max.index ? 0 : value
            let term2 = quotient
            let term3 = j < remainder ? 1 : 0
            select (byte)(term1 + term2 + term3);

        var redistributed = query.ToImmutableArray();
        return redistributed;
    }
}