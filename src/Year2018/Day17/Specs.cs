namespace AdventOfCode.Year2018.Day17;

public class Specs
{
    string[] input = new[]
    {
            "x=495, y=2..7",
            "y=7, x=495..501",
            "x=501, y=3..7",
            "x=498, y=2..4",
            "x=506, y=1..2",
            "x=498, y=10..13",
            "x=504, y=10..13",
            "y=13, x=498..504"
        };

    [Fact]
    public void ParseTest1()
    {
        var range = Grid.ParseLine(input[0]).ToArray();
        Assert.Equal(new[] { (495, 2), (495, 3), (495, 4), (495, 5), (495, 6), (495, 7) }, range);
    }
    [Fact]
    public void ParseTest2()
    {
        var range = Grid.ParseLine(input[1]).ToArray();
        Assert.Equal(new[] { (495, 7), (496, 7), (497, 7), (498, 7), (499, 7), (500, 7), (501, 7) }, range);
    }

    [Fact]
    public void Grid_Parse()
    {
        var grid = Grid.Parse(input);

        var expected = initial;


        Assert.Equal(494, grid.MinX);
        Assert.Equal(507, grid.MaxX);
        Assert.Equal(expected, grid.ToString());
    }
    const string initial = @"......+.......
............#.
.#..#.......#.
.#..#..#......
.#..#..#......
.#.....#......
.#.....#......
.#######......
..............
..............
....#.....#...
....#.....#...
....#.....#...
....#######...";

    string expected = @"......+.......
......|.....#.
.#..#||||...#.
.#..#~~#|.....
.#..#~~#|.....
.#~~~~~#|.....
.#~~~~~#|.....
.#######|.....
........|.....
...|||||||||..
...|#~~~~~#|..
...|#~~~~~#|..
...|#~~~~~#|..
...|#######|..";

    [Fact]
    public void Grid_Simulate()
    {
        var grid = Grid.Parse(input);
        Assert.Equal(initial, grid.ToString());
        grid.Simulate();
        Assert.Equal(expected, grid.ToString());
    }

    [Fact]
    public void MoveDown()
    {
        (int x, int y) pos = (0, 0);
        Func<(int x, int y), bool> predicate = p => p.y + 1 < 3;
        var last = pos.MoveDown(predicate).Last();
        Assert.Equal((0, 2), last);
    }

    [Fact]
    public void TestPart1()
    {
        var result = AoC201817.Part1(input);
        Assert.Equal(57, result);
    }

    [Fact]
    public void TestPart2()
    {
        var result = AoC201817.Part2(input);
        Assert.Equal(29, result);
    }
}
