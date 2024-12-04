namespace AdventOfCode.Year2024.Day04;

public class AoC202404(string[] input)
{
    readonly int X = input[0].Length;
    readonly int Y = input.Length;
    readonly Range rX = 0..input[0].Length;
    readonly Range rY = 0..input.Length;

    public AoC202404() : this(Read.InputLines()) {}

    public int Part1() => (
        from l in input.Index()
        from c in l.Item.Index()
        where c.Item == 'X'
        from d in Enum.GetValues<Direction>()
        let r = Get(c.Index, l.Index, d)
        where r is ('X', 'M', 'A', 'S')
        select r
        ).Count();

    enum Direction
    {
        N, NE, E, SE, S, SW, W, NW
    }

    (char, char, char, char) Get(int x, int y, Direction d) => d switch
    {
        Direction.E  when IsValidX(x + 3)       => (input[y][x], input[y    ][x + 1], input[y    ][x + 2], input[y    ][x + 3]),
        Direction.W  when IsValidX(x - 3)       => (input[y][x], input[y    ][x - 1], input[y    ][x - 2], input[y    ][x - 3]),
        Direction.N  when IsValidY(y - 3)       => (input[y][x], input[y - 1][x    ], input[y - 2][x    ], input[y - 3][x    ]),
        Direction.S  when IsValidY(y + 3)       => (input[y][x], input[y + 1][x    ], input[y + 2][x    ], input[y + 3][x    ]),
        Direction.NE when IsValid(x + 3, y - 3) => (input[y][x], input[y - 1][x + 1], input[y - 2][x + 2], input[y - 3][x + 3]),
        Direction.NW when IsValid(x - 3, y - 3) => (input[y][x], input[y - 1][x - 1], input[y - 2][x - 2], input[y - 3][x - 3]),
        Direction.SE when IsValid(x + 3, y + 3) => (input[y][x], input[y + 1][x + 1], input[y + 2][x + 2], input[y + 3][x + 3]),
        Direction.SW when IsValid(x - 3, y + 3) => (input[y][x], input[y + 1][x - 1], input[y + 2][x - 2], input[y + 3][x - 3]),
        _ => (' ', ' ', ' ', ' ')
    };

    public int Part2() => (
        from l in input.Index()
        from c in l.Item.Index()
        where c.Item == 'A'
        let y = l.Index let x = c.Index
        where IsValid(x - 1, y - 1) && IsValid(x + 1, y + 1)
        let nwse = (input[y - 1][x - 1], input[y + 1][x + 1])
        let swne = (input[y - 1][x + 1], input[y + 1][x - 1])
        where (
               nwse is ('M', 'S') or ('S', 'M') 
            && swne is ('M', 'S') or ('S', 'M')
        )
        select c).Count();

    private bool IsValid(int x, int y) => IsValidX(x) && IsValidY(y);
    private bool IsValidX(int x) => rX.Contains(x, X);
    private bool IsValidY(int y) => rY.Contains(y, Y);
}

static class Ex
{
    public static bool Contains(this Range r, int value, int length)
    {
        var (start, end) = (r.Start.GetOffset(length), r.End.GetOffset(length));
        return value >= start && value < end;
    }
}

public class AoC202404Tests
{
    private readonly AoC202404 sut;
    public AoC202404Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202404(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(18, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(9, sut.Part2());
    }
}