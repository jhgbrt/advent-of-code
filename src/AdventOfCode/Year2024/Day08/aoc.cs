namespace AdventOfCode.Year2024.Day08;
public class AoC202408(string[] input)
{
    public AoC202408() : this(Read.InputLines()) {}

    bool IsValid(Coordinate p) => p.x >= 0 && p.x < input[0].Length && p.y >= 0 && p.y < input.Length;

    List<(Coordinate, Coordinate)> antennas = (
        from line in input.Index()
        from value in line.Item.Index()
        where value.Item != '.'
        let c = new Coordinate(value.Index, line.Index)
        group c by value.Item into g
        from p in g.Combinations()
        select p
        ).ToList();

    public int Part1() => (from p in antennas
                           from a in GetAntinodes1(p)
                           select a).Distinct().Count();
    public int Part2() => (from p in antennas
                           from a in GetEquidistantPoints(p)
                           select a).Distinct().Count();

    IEnumerable<Coordinate> GetAntinodes1((Coordinate c1, Coordinate c2) pair)
    {
        var (c1, c2) = pair;
        var delta = c2 - c1;
        if (IsValid(c1 - delta)) yield return (c1 - delta);
        if (IsValid(c2 + delta)) yield return (c2 + delta);
    }
    IEnumerable<Coordinate> GetEquidistantPoints((Coordinate c1, Coordinate c2) pair)
    {
        var (c1, c2) = pair;
        var delta = c2 - c1;
        return GetEquidistantPoints(c1, -1*delta).Concat(GetEquidistantPoints(c2, delta));
    }
    IEnumerable<Coordinate> GetEquidistantPoints(Coordinate c, Delta d)
    {
        while (IsValid(c))
        {
            yield return c;
            c += d;
        }
    }


}

public class AoC202408_1_Tests
{
    private readonly AoC202408 sut;
    public AoC202408_1_Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines(1);
        sut = new AoC202408(input);
    }

    [Fact]
    public void TestParsing()
    {
        
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(14, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(34, sut.Part2());
    }
}

public class AoC202408_2_Tests
{
    private readonly AoC202408 sut;
    public AoC202408_2_Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines(2);
        sut = new AoC202408(input);
    }

    [Fact]
    public void TestParsing()
    {

    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(2, sut.Part1());
    }

}

readonly record struct Coordinate(int x, int y)
{
    public static Delta operator -(Coordinate left, Coordinate right) => new(left.x - right.x, left.y - right.y);
    public static Coordinate operator +(Coordinate left, Delta p) => new(left.x + p.dx, left.y + p.dy);
    public static Coordinate operator -(Coordinate left, Delta p) => new(left.x - p.dx, left.y - p.dy);
}

readonly record struct Delta(int dx, int dy)
{
    public static Delta operator *(int n, Delta d) => new(d.dx * n, d.dy * n);
}
