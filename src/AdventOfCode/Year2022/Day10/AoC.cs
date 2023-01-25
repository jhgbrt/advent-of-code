namespace AdventOfCode.Year2022.Day10;
public class AoC202210
{
    static string[] input = Read.InputLines();
    public long Part1() => (
        from c in Cycles().Select((x, i) => (x, n: i + 1))
        where new[] { 20, 60, 100, 140, 180, 220 }.Contains(c.n)
        select c.x * c.n
    ).Sum();

    private IEnumerable<int> Cycles()
    {
        var x = 1;
        var instructions = from line in input
                           let instruction = ToInstruction(line)
                           select instruction;

        foreach (var (add, n, line) in instructions)
        {
            for (int i = 0; i < n; i++)
            {
                yield return x;
            }
            x += add;
        }
    }

    public string Part2() => (
        from cycle in Cycles().Select((x, i) => (x, i))
        let linenr = cycle.i / 40
        let position = cycle.i % 40
        let c = Range(cycle.x - 1, 3).Contains(position) ? '#' : '.'
        group c by linenr
        ).Aggregate(new StringBuilder(), 
        (sb, line) => sb.AppendLine(new string(line.ToArray()))
        ).ToString().DecodePixels(AsciiFontSize._4x6);

    private (int add, int n, string instruction) ToInstruction(string line) => line[0..4] switch
    {
        "noop" => (0, 1, line),
        "addx" => (int.Parse(line[5..]), 2, line),
        _ => throw new NotImplementedException("unrecognized instruction")
    };

}


