namespace AdventOfCode.Year2024.Day04;

public class AoC202404(string[] input)
{
    public AoC202404() : this(Read.InputLines()) {}


    public int Part1() => (from line in input.Index()
                           from c in line.Item.Index()
                           where c.Item == 'X'
                           from d in Enum.GetValues<Direction>()
                           let r = Get(c.Index, line.Index, d)
                           where r is ('X', 'M', 'A', 'S')
                           select r).Count();

    enum Direction
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }
    (int a, int b, int c, int d) Get(int x, int y, Direction d) => d switch
    {
        Direction.E when x + 3 < input[0].Length => (input[y][x], input[y][x + 1], input[y][x + 2], input[y][x + 3]),
        Direction.W when x - 3 >= 0 => (input[y][x], input[y][x - 1], input[y][x - 2], input[y][x - 3]),
        Direction.N when y - 3 >= 0 => (input[y][x], input[y - 1][x], input[y - 2][x], input[y - 3][x]),
        Direction.S when y + 3 < input.Length => (input[y][x], input[y + 1][x], input[y + 2][x], input[y + 3][x]),
        Direction.NE when x + 3 < input[0].Length && y - 3 >= 0 => (input[y][x], input[y - 1][x + 1], input[y - 2][x + 2], input[y - 3][x + 3]),
        Direction.NW when x - 3 >= 0 && y - 3 >= 0 => (input[y][x], input[y - 1][x - 1], input[y - 2][x - 2], input[y - 3][x - 3]),
        Direction.SE when x + 3 < input[0].Length && y + 3 < input.Length => (input[y][x], input[y + 1][x + 1], input[y + 2][x + 2], input[y + 3][x + 3]),
        Direction.SW when x - 3 >= 0 && y + 3 < input.Length => (input[y][x], input[y + 1][x - 1], input[y + 2][x - 2], input[y + 3][x - 3]),
        _ => (' ', ' ', ' ', ' ')
    };

    public int Part2() => (from line in input.Index()
                           from c in line.Item.Index()
                           where IsMAS(c.Index, line.Index)
                           select 1).Sum();

    bool IsMAS(int x, int y)
    {
        if (x < 1 || x >= input[0].Length - 1 || y < 1 || y >= input.Length - 1)
        {
            return false;
        }

        var diagonal1 = (input[y - 1][x - 1], input[y][x], input[y + 1][x + 1]);
        var diagonal2 = (input[y - 1][x + 1], input[y][x], input[y + 1][x - 1]);
        return diagonal1 is ('M', 'A', 'S') or ('S', 'A', 'M') && diagonal2 is ('M', 'A', 'S') or ('S', 'A', 'M');
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