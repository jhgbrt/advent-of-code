namespace AdventOfCode.Year2019.Day06;

public class AoCImpl : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoCImpl));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);
    public static int Part1(string[] input)
    {
        var graph = input.CreateGraph();
        return graph.Vertices
            .Select(v => graph.CountDistance("COM", v))
            .Sum();
    }

    public static int Part2(string[] input)
        => input.CreateGraph().CountDistance("YOU", "SAN") - 2;
}