namespace AdventOfCode.Year2020.Day15;

public class AoCImpl : AoCBase
{
    static int[] input = new[] { 0, 1, 4, 13, 15, 12, 16 };
    public override object Part1() => Run(input, 2020);
    public override object Part2() => Run(input, 30000000);

    internal static long Run(int[] input, int max)
    {
        var dic = input.Select((n, i) => (n, i)).ToDictionary(x => x.n, x => (turn_1: x.i, turn_2: x.i));
        int last = input.Last();
        for (var i = input.Length; i < max; i++)
        {
            var next = dic[last].turn_1 - dic[last].turn_2;
            var previous = dic.ContainsKey(next) ? dic[next].turn_1 : i;
            dic[next] = (i, previous);
            last = next;
        }
        return last;
    }
}