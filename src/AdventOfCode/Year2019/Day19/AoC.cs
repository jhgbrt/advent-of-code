using AdventOfCode.Common.Collections;

using System.Collections.Concurrent;

namespace AdventOfCode.Year2019.Day19;
public class AoC201919
{
    public AoC201919() : this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    long[] program;

    public AoC201919(string[] input, TextWriter writer)
    {
        program = input.First().Split(',').Select(long.Parse).ToArray();
        this.writer = writer;
        Print(50);
    }

    public long Part1() => (from c in Coordinates(50)
                            select IsCovered(c.x, c.y) ? 1 : 0).Sum();

    bool IsCovered(long x, long y) => new IntCode(program).Run((x,y).AsEnumerable()).First() == 1;

    IEnumerable<(int x, int y)> Coordinates(int n)
    {
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x ++)
            {
                yield return (x, y);
            }
    }

    public void Print(int n)
    {

        int x1 = 0;
        while (!IsCovered(x1, 100)) x1++;
        int x2 = x1;
        while (IsCovered(x2, 100)) x2++;
        var c1 = (100 - x1);
        var c2 = (100 - x2);

        for (int y = 0; y < n; ++y)
        {
            for (int x = 0; x < n; ++x)
            {
                writer.Write(IsCovered(x, y) ? '#' : '.');
            }
            writer.WriteLine();
        }
    }

    public int Part2()
    {

        var (x, y) = FindSquare(100);
        return 10000 * x + y;
    }

    (int x, int y) FindSquare(int n)
    {
        Range beam = 0..0;
        var queue = new FixedSizedQueue<(int y, Range range)>(n);

        var y = 1000;
        while (queue.Count < n || queue.Peek().range.End != (beam.Start + n))
        {
            beam = GetBeam(beam, y, n);
            if (beam.Length >= n)
            {
                queue.Enqueue((y, beam));
            }
            y++;
        }
        return (beam.Start, queue.Peek().y);
    }

    Range GetBeam(Range previous, int y, int n)
    {
        bool result = false;
        int x = previous.Start;
        while (!result && x < y)
        {
            result = IsCovered(x, y);
            x++;
        }

        if (!result)
        {
            return 0..0;
        }

        var start = x - 1;
        x += n;

        if (!(result = IsCovered(x, y)))
        {
            return start..start;
        }

        while (result)
        {
            x++;
            result = IsCovered(x, y);
        }
        return start..x;
    }

    readonly record struct Range(int Start, int End)
    {
        public int Length => End - Start - 1;
        public static implicit operator Range(System.Range r) => new(r.Start.Value, r.End.Value);
    }
}

public class AoC201919Tests
{
    private readonly AoC201919 sut;
    public AoC201919Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC201919(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        sut.Print(100);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(211, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(8071006, sut.Part2());
    }
}