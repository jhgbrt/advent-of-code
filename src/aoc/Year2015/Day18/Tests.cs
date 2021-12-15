namespace AdventOfCode.Year2015.Day18;

public class Tests
{
    Grid grid;
    public Tests()
    {
        var lines = Read.SampleLines();
        grid = new Grid((
            from y in Enumerable.Range(0, lines.Length)
            from x in Enumerable.Range(0, lines[y].Length)
            where lines[y][x] == '#'
            select new Coordinate(x, y)
            ).ToHashSet(), lines.Length);

    }

    [Fact]
    public void ToString_ReturnsOriginalGrid()
    {
        var text = Read.SampleText();
        Assert.Equal(text, grid.ToString());
    }

    [Fact]
    public void NeighboursOf_0_0()
    {
        var start = new Coordinate(0, 0);
        grid.Neighbours(start);
        Assert.Equal(new Coordinate[] { new(0, 1), new(1, 0), new(1, 1) }, grid.Neighbours(start).OrderBy(x => x.x).ThenBy(x => x.y));
    }
    [Fact]
    public void NeighboursOf_1_0()
    {
        var start = new Coordinate(1, 0);
        grid.Neighbours(start);
        Assert.Equal(new Coordinate[] { new(0, 0), new(0, 1), new(1, 1), new(2, 0), new(2, 1) }, grid.Neighbours(start).OrderBy(x => x.x).ThenBy(x => x.y));
    }
    [Fact]
    public void NeighboursOf_4_3()
    {
        var start = new Coordinate(4, 3);
        grid.Neighbours(start);
        Assert.Equal(new Coordinate[] { new(3, 2), new(3, 3), new(3, 4), new(4, 2), new(4, 4), new(5, 2), new(5, 3), new(5, 4) }, grid.Neighbours(start).OrderBy(x => x.x).ThenBy(x => x.y));
    }
}