namespace AdventOfCode.Year2017.Day16;

public class AoC201716 : AoCBase
{
    static Dancer dancer = new Dancer(Read.InputStream(typeof(AoC201716)));
    static string initial = "abcdefghijklmnop";
    public override object Part1() => dancer.Run(initial);
    public override object Part2() => dancer.Run(initial, 1000000000);

}