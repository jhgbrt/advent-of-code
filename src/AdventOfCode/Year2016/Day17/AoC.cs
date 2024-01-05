using AdventOfCode.Common;

using System;

namespace AdventOfCode.Year2016.Day17;

public class AoC201617
{
    static string input = Read.InputLines()[0];

    public string Part1() => Compute(input, true);
    public object Part2() => Compute(input, false);

    private string Compute(string passcode, bool shortest)
    {
        var queue = new Queue<(Coordinate c, string path)>();
        queue.Enqueue((new(0, 0), ""));
        var target = new Coordinate(3, 3);
        int longest = 0;

        while (queue.Any())
        {
            var (c, path) = queue.Dequeue();
            if (c == target)
            {
                if (shortest)
                    return path;
                else
                    longest = Max(longest, path.Length);
            }
            else
            {
                var hash = MD5Hash.Compute(passcode + path);

                foreach (var (n, d) in c.Next(hash))
                {
                    queue.Enqueue((n, path + d));
                }
            }
        }
        return longest.ToString();
    }
}
readonly record struct Coordinate(int x, int y)
{
    public IEnumerable<(Coordinate, string)> Next(string hash)
    {
        if (hash[0] is >= 'b' and <= 'f' && y > 0) yield return (this with { y = y - 1 }, "U");
        if (hash[1] is >= 'b' and <= 'f' && y < 3) yield return (this with { y = y + 1 }, "D");
        if (hash[2] is >= 'b' and <= 'f' && x > 0) yield return (this with { x = x - 1 }, "L");
        if (hash[3] is >= 'b' and <= 'f' && x < 3) yield return (this with { x = x + 1 }, "R");
    }
}
