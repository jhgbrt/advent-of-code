namespace AdventOfCode.Year2016.Day18;

public class AoC201618
{
    static string input = Read.InputLines()[0];

    const char SAFE = '.';
    const char TRAP = '^';

    public object Part1() => Compute(40);
    public object Part2() => Compute(400000);

    private static object Compute(int rows)
    {
        var previous = new char[input.Length + 2];
        var next = new char[input.Length + 2];

        Array.Copy(input.ToCharArray(), 0, previous, 1, input.Length);
        previous[0] = previous[^1] = next[0] = next[^1] = SAFE;

        int count = previous.Count(c => c == SAFE) - 2;
        foreach (var i in Range(1, rows-1))
        {
            foreach (var j in Range(1, input.Length))
            {
                var d = (previous[j - 1], previous[j], previous[j + 1]) switch
                {
                    (TRAP, TRAP, SAFE) or (SAFE, TRAP, TRAP) => TRAP,
                    (TRAP, SAFE, SAFE) or (SAFE, SAFE, TRAP) => TRAP,
                    _ => SAFE
                };
                if (d == SAFE) count++;
                next[j] = d;
            }
            (previous, next) = (next, previous);
        }

        return count;
    }

}