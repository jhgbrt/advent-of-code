namespace AdventOfCode.Year2017.Day17;

public class AoCImpl : AoCBase
{
    const int input = 377;
    public override object Part1()
    {
        var result = Spinlock.Find(input, 2017);
        return result.buffer[result.index + 1];
    }
    //public override object Part1() => Spinlock.FindFast(input, 2017);
    public override object Part2() => Spinlock.FindFast(input, 50_000_000);

}