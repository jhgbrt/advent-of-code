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
public class Spinlock
{
    public static (int index, IList<int> buffer) Find(int steps, int maxValue)
    {
        var currentPosition = 0;
        var currentValue = 0;
        var buffer = new List<int>(maxValue + 1) { 0 };
        for (int i = 0; i < maxValue; i++)
        {
            currentPosition = (currentPosition + steps) % buffer.Count;
            currentValue++;
            currentPosition++;
            buffer.Insert(currentPosition, currentValue);
        }
        return (currentPosition, buffer);
    }

    public static int FindFast(int steps, int maxValue)
    {
        var currentPosition = 0;
        var n = 0;
        var result = 0;
        while (n < maxValue)
        {
            var diff = (n - currentPosition) / steps + 1;
            n += diff;
            currentPosition = (currentPosition + diff * (steps + 1) - 1) % n;
            if (currentPosition == 0 && n < maxValue) result = n;
            currentPosition++;
        }
        return result;
    }
}