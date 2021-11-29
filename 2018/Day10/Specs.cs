namespace AdventOfCode.Year2018.Day10;

public class Specs
{
    private const string Expected = @"#...#..###
#...#...#.
#...#...#.
#####...#.
#...#...#.
#...#...#.
#...#...#.
#...#..###";

    string[] input = new[]
    {
            "position=< 9,  1> velocity=< 0,  2>",
            "position=< 7,  0> velocity=<-1,  0>",
            "position=< 3, -2> velocity=<-1,  1>",
            "position=< 6, 10> velocity=<-2, -1>",
            "position=< 2, -4> velocity=< 2,  2>",
            "position=<-6, 10> velocity=< 2, -2>",
            "position=< 1,  8> velocity=< 1, -1>",
            "position=< 1,  7> velocity=< 1,  0>",
            "position=<-3, 11> velocity=< 1, -2>",
            "position=< 7,  6> velocity=<-1, -1>",
            "position=<-2,  3> velocity=< 1,  0>",
            "position=<-4,  3> velocity=< 2,  0>",
            "position=<10, -3> velocity=<-1,  1>",
            "position=< 5, 11> velocity=< 1, -2>",
            "position=< 4,  7> velocity=< 0, -1>",
            "position=< 8, -2> velocity=< 0,  1>",
            "position=<15,  0> velocity=<-2,  0>",
            "position=< 1,  6> velocity=< 1,  0>",
            "position=< 8,  9> velocity=< 0, -1>",
            "position=< 3,  3> velocity=<-1,  1>",
            "position=< 0,  5> velocity=< 0, -1>",
            "position=<-2,  2> velocity=< 2,  0>",
            "position=< 5, -2> velocity=< 1,  2>",
            "position=< 1,  4> velocity=< 2,  1>",
            "position=<-2,  7> velocity=< 2, -2>",
            "position=< 3,  6> velocity=<-1, -1>",
            "position=< 5,  0> velocity=< 1,  0>",
            "position=<-6,  0> velocity=< 2,  0>",
            "position=< 5,  9> velocity=< 1, -2>",
            "position=<14,  7> velocity=<-2,  0>",
            "position=<-3,  6> velocity=< 2, -1>"
        };

    [Theory]
    [InlineData("position=<-3,  6> velocity=< 2, -1>", -3, 6, 2, -1)]
    [InlineData("position=<-31,  -6> velocity=< 22, -11>", -31, -6, 22, -11)]
    [InlineData("position=<33,6> velocity=<-22, 11>", 33, 6, -22, 11)]
    public void ParseTest(string input, int x, int y, int dx, int dy)
    {
        var point = Point.Parse(input);
        Assert.Equal(x, point.X);
        Assert.Equal(y, point.Y);
        Assert.Equal(dx, point.Vx);
        Assert.Equal(dy, point.Vy);
    }

    [Fact]
    public void Point_Equals()
    {
        var point1 = new Point(1, 2, 3, 4);
        var point2 = new Point(1, 2, 3, 4);
        Assert.True(point1 == point2);
    }
    [Fact]
    public void Point_HashCode()
    {
        var point1 = new Point(1, 2, 3, 4);
        var point2 = new Point(1, 2, 3, 4);
        Assert.True(point1.GetHashCode() == point2.GetHashCode());
    }

    [Fact]
    public void Point_Move_MovesPointWithVelocity()
    {
        var point = new Point(3, 9, 1, -2).Move(3);
        Assert.Equal(new Point(6, 3, 1, -2), point);
        point = point.Move(-3);
        Assert.Equal(new Point(3, 9, 1, -2), point);
    }

    [Fact]
    public void Grid()
    {
        var points = (from s in input select Point.Parse(s)).ToArray();
        var grid = new Grid(points);
        Assert.Equal((22, 16), (grid.Width, grid.Height));
    }

    [Fact]
    public void Grid_Tick()
    {
        var points = (from s in input select Point.Parse(s)).ToArray();
        var grid = new Grid(points).Move(1);
        Assert.Equal((18, 12), (grid.Width, grid.Height));
    }

    [Fact]
    public void FindMessage()
    {
        var points = (from s in input select Point.Parse(s)).ToArray();
        var grid = new Grid(points.ToArray()).FindGridWithLowestHeight();
        Assert.Equal(Expected, grid.ToString());
    }

    [Fact]
    public void TestPart1()
    {
        var result = AoC.ToGrid(input).FindGridWithLowestHeight().ToString();
        Assert.Equal(Expected, result);
    }

    [Fact]
    public void TestPart2()
    {
        var result = AoC.Part2(input);
        Assert.Equal(3, result);
    }
}
