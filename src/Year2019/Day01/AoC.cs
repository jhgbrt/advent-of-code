namespace AdventOfCode.Year2019.Day01;

public class AoC201901 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC201901));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);
    public static int Part1(string[] input) => input.Select(int.Parse).Select(CalculateFuel1).Sum();

    public static int Part2(string[] input) => input.Select(int.Parse).Select(CalculateFuel2).Sum();
    public static int CalculateFuel1(int mass) => mass / 3 - 2;
    public static int CalculateFuel2(int mass) => Fuel(mass).Sum();

    static IEnumerable<int> Fuel(int mass)
    {
        var fuel = mass;
        while (true)
        {
            fuel = CalculateFuel1(fuel);
            if (fuel < 0) break;
            yield return fuel;
        }
    }
}