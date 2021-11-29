namespace AdventOfCode.Year2018.Day18;

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));


    public static int Part1(string[] input) => new Grid(input).Step(10).Value;

    public static int Part2(string[] input)
    {
        var grid = new Grid(input);

        var items = new Dictionary<string, int>();

        string gridAsString = string.Empty;

        int i = 0;
        while (true)
        {
            grid = grid.Step();
            i++;
            gridAsString = grid.ToString();
            if (items.ContainsKey(gridAsString))
                break;
            items[gridAsString] = i;
        }

        var patternStartsAt = items[grid.ToString()];

        var patternSize = items.Count - patternStartsAt + 1;

        var nofsteps = patternStartsAt + (1000000000 - patternStartsAt) % patternSize;

        return new Grid(input).Step(nofsteps).Value;
    }
}

enum Acre
{
    Open = '.',
    Tree = '|',
    Lumber = '#'
}

