namespace AdventOfCode.Year2018.Day01;

public class AoC201801
{
    static string[] input = Read.InputLines();

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);

    public static int Part1(string[] input) => input.Select(int.Parse).Sum();

    public static int Part2(string[] input)
    {
        var ints = input.Select(int.Parse);
        var frequency = 0;
        var hashSet = new HashSet<int>();
        ints.EndlessRepeat().TakeWhile(i =>
        {
            hashSet.Add(frequency);
            frequency += i;
            return !hashSet.Contains(frequency);
        }).Last();
        return frequency;
    }
}