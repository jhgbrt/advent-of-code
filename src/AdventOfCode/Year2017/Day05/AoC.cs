namespace AdventOfCode.Year2017.Day05;

public class AoC201705
{
    public static int[] input = Read.InputLines().Select(int.Parse).ToArray();

    public object Part1() => CalculateJumps(input, v => 1);
    public object Part2() => CalculateJumps(input, v => v >= 3 ? -1 : 1);

    public static int CalculateJumps(ReadOnlySpan<int> input, Func<int, int> step)
    {
        var copy = input.ToArray();
        var steps = 0;
        var i = 0;
        while (i >= 0 && i < input.Length)
        {
            steps++;
            var j = i;
            var v = copy[i];
            i += v;
            copy[j] += step(v);
        }
        return steps;
    }
}
