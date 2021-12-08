namespace AdventOfCode.Year2021.Day08;

public class AoC202108 : AoCBase
{
    static string[] input = Read.SampleLines(typeof(AoC202108));
    public override object Part1()
    {
        var q = from p in input
                let fragment = p.Split('|').Last()
                from value in fragment.Split(' ')
                where value.Length is 2 or 3 or 4 or 7
                select value.Length switch
                {
                    2 => 1,
                    3 => 7,
                    4 => 4,
                    7 => 8,
                    _ => throw new Exception()
                };

        return q.Count();



    }
    public override object Part2() => -1;
}
