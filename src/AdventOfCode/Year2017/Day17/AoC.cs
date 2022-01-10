namespace AdventOfCode.Year2017.Day17;

public class AoC201717
{
    const int input = 377;
    public object Part1()
    {
        var result = Spinlock.Find(input, 2017);
        return result.buffer[result.index + 1];
    }
    //public object Part1() => Spinlock.FindFast(input, 2017);
    public object Part2() => Spinlock.FindFast(input, 50_000_000);

}