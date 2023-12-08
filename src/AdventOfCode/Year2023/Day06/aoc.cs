namespace AdventOfCode.Year2023.Day06;
public class AoC202306
{
    static string[] input = Read.InputLines();
    IEnumerable<int> times = input[0].Split(':', StringSplitOptions.TrimEntries)[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);
    IEnumerable<int> distances = input[1].Split(':', StringSplitOptions.TrimEntries)[1].Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse);

    public int Part1() => (from z in times.Zip(distances)
                           let t = z.First
                           let r = z.Second
                           let n = (
                               from x in Range(1, z.First - 1)
                               let d = (t - x) * x
                               where d > r
                               select x
                               ).Count()
                           select n).Aggregate(1, (a, b) => a * b);

    long time = long.Parse(new(input[0].Where(char.IsDigit).ToArray()));
    long record = long.Parse(new(input[1].Where(char.IsDigit).ToArray()));

    public long Part2()
    {
        var lower = Range(1L, time - 1).First(speed => (time - speed) * speed > record);
        var upper = Range(time, 1, -1).First(speed => (time - speed) * speed > record);
        return upper - lower + 1;
    } 
    static IEnumerable<long> Range(long start, long length, int step = 1)
    {
        for (var i = start; i <= start + length; i += step)
        {
            yield return i;
        }
    }
}


