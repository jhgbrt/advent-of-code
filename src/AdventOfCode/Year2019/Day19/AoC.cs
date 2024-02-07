namespace AdventOfCode.Year2019.Day19;
public class AoC201919
{
    public AoC201919() : this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    long[] program;
    IntCode intcode;

    public AoC201919(string[] input, TextWriter writer)
    {
        program = input.First().Split(',').Select(long.Parse).ToArray();
        intcode = new IntCode(program);
        this.writer = writer;
    }

    public long Part1() => (from c in Coordinates(50)
                            select Run(c.x, c.y)).Sum();

    long Run(long x, long y) => new IntCode(program).Run(new[] { x, y }).First();

    IEnumerable<(int x, int y)> Coordinates(int n)
    {
        for (int x = 0; x < n; x ++)
        {
            for (int y = 0; y < n; y ++)
            {
                yield return (x, y);
            }
        }
    }

    public void Print(int n)
    {
        for (int y = 0; y < n; ++y)
        {
            for (int x = 0; x < n; ++x)
            {
                writer.Write(Run(x, y) == 1 ? '#' : '.');
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
        var startX = 0;
        var endX = new int[2000];

        for (var y = 1000; ; y++) // 1000 is a guess; may not work for all inputs
        {
            var r = 0L;
            var x = startX;

            while (r == 0 && x < y + 10)
            {
                r = Run(x, y);
                x++;
            }

            if (r != 1)
                continue;

            startX = x - 1;
            x += n;
            while (r == 1)
            {
                r = Run(x, y);
                x++;
            }
            endX[y] = x - 1;

            if (y >= n && endX[y - (n - 1)] == (startX + n))
            {
                return (startX, y - (n - 1));
            }
        }

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