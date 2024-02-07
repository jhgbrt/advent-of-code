using AdventOfCode.Common.Collections;
using AdventOfCode.Tests;

using System.Collections.Concurrent;

namespace AdventOfCode.Year2019.Day19;
public class AoC201919
{
    public AoC201919() : this(Read.InputLines(), Console.Out) { }
    readonly TextWriter writer;
    long[] program;

    public AoC201919(string[] input, TextWriter writer)
    {
        program = input.First().Split(',').Select(long.Parse).ToArray();
        this.writer = writer;
    }

    public long Part1() => (from c in Coordinates(50)
                            select Run(c.x, c.y) ? 1 : 0).Sum();

    bool Run(long x, long y) => new IntCode(program).Run((x, y).AsEnumerable()).First() == 1;

    IEnumerable<(int x, int y)> Coordinates(int n)
    {
        for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                yield return (x, y);
            }
    }

    private (decimal m1, decimal m2) CalculateSlopes()
    {
        int y = 1000;
        int x1 = 0;
        while (!Run(x1, y)) x1++;
        int x2 = x1;
        while (Run(x2, y)) x2++;
        return ((decimal)y / x2, (decimal)y / x1);
    }

    public void Print(int n)
    {

        for (int y = 0; y < n; ++y)
        {
            for (int x = 0; x < n; ++x)
            {
                writer.Write(Run(x, y) ? '#' : '.');
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

        // estimation for the starting point
        var (m1, m2) = CalculateSlopes();
        var x1 = ((m1 * n - 1) + n - 1) / (m2 - m1);
        var y1 = (m2 * x1) - (n - 1);

        var y = (int)y1;
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
            result = Run(x, y);
            x++;
        }

        if (!result)
        {
            return 0..0;
        }

        var start = x - 1;
        x += n;

        if (!(result = Run(x, y)))
        {
            return start..start;
        }

        while (result)
        {
            x++;
            result = Run(x, y);
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