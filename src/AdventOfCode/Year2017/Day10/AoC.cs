namespace AdventOfCode.Year2017.Day10;

public class AoC201710
{
    static string input = "206,63,255,131,65,80,238,157,254,24,133,2,16,0,1,3";
    public object Part1()
    {
        var result = KnotsHash.Hash(input.Split(',').Select(byte.Parse).ToArray());
        var value = result[0] * result[1];
        return value;
    }
    public object Part2() => KnotsHash.Hash(input);
}