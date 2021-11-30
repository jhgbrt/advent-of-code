namespace AdventOfCode.Year2020.Day25;

public class AoCImpl : AoCBase
{
    public static string[] input = Read.InputLines(typeof(AoCImpl));

    public override object Part1()
    {
        var (key1, key2) = (2084668L, 3704642L);
        long prime = 20201227, value = 1, result = 1;
        while (value != key2)
        {
            (value, result) = (value * 7 % prime, result * key1 % prime);
        }
        return result;
    }
    public override object Part2() => -1;

}