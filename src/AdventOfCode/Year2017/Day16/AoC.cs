namespace AdventOfCode.Year2017.Day16;

public class AoC201716
{
    static Dancer dancer = new Dancer(Read.InputStream());
    static string initial = "abcdefghijklmnop";
    public object Part1() => dancer.Run(initial);
    public object Part2() => dancer.Run(initial, 1000000000);

}