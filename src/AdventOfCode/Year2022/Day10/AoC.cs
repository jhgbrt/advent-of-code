namespace AdventOfCode.Year2022.Day10;
public class AoC202210
{
    static string[] input = Read.InputLines();
    public long Part1() => (from c in Cycles().Select((x, i) => (x, n: i+1))
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

    [Fact]
    public void Part1Test()
    {
        Assert.Equal(13860, Part1());
    }

    public string Part2() 
    {
        var q = from c in Cycles().Select((x, i) => (x, i))
                group c by c.i / 40;

        var sb = new StringBuilder();
        var sprite = 0;
        foreach (var line in q)
        {
            var lineb = new StringBuilder();
            foreach (var pixel in line)
            {
                sprite = pixel.x;
                var cycle = pixel.i;
                var drawingpos = pixel.i % 40;
                if (Range(sprite - 1, 3).Contains(drawingpos))
                    lineb.Append('#');
                else
                    lineb.Append('.');
            }
            sb.AppendLine(lineb.ToString());
            lineb.Clear();
        }
        Console.WriteLine(sb.ToString());
        return PixelFontDecoder.DecodePixels(sb.ToString(), 5);
    }

    private (int add, int n, string instruction) ToInstruction(string line) => line[0..4] switch
    {
        "noop" => (0, 1, line),
        "addx" => (int.Parse(line[5..]), 2, line),
        _ => throw new NotImplementedException("unrecognized instruction")
    };

}


