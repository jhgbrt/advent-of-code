﻿namespace AdventOfCode.Year2020.Day17.Part1;

using static State;
public static class Runner
{
    public static int Run()
    {
        var grid = Grid.FromFile(@"input.txt");

        for (int i = 0; i < 6; i++)
        {
            grid = grid.Cycle();
        }

        return grid.ActiveCells.Count;
    }
}

record Coordinate(int x, int y, int z)
{
    public override string ToString() => $"({x},{y},{z})";
    public IEnumerable<Coordinate> Neighbors() => from dx in Enumerable.Range(-1, 3)
                                                  from dy in Enumerable.Range(-1, 3)
                                                  from dz in Enumerable.Range(-1, 3)
                                                  where (dx, dy, dz) != (0, 0, 0)
                                                  select new Coordinate(x + dx, y + dy, z + dz);
}

record Grid(ImmutableHashSet<Coordinate> ActiveCells)
{
    public static Grid FromFile(string fileName) => FromLines(Read.InputLines());
    public static Grid FromLines(IEnumerable<string> lines) => new Grid((
                from lineindex in lines.Select((line, y) => (line, y))
                from charindex in lineindex.line.Select((c, x) => (c, x))
                where charindex.c == '#'
                select new Coordinate(charindex.x, lineindex.y, z: 0)
                ).ToImmutableHashSet()
            );

    private IEnumerable<int> All(Func<Coordinate, int> project) => ActiveCells.Select(project);
    private Coordinate Min => new(All(c => c.x).Min(), All(c => c.y).Min(), All(c => c.z).Min());
    private Coordinate Max => new(All(c => c.x).Max(), All(c => c.y).Max(), All(c => c.z).Max());

    public State this[Coordinate c] => ActiveCells.Contains(c) ? Active : Inactive;

    public IEnumerable<Coordinate> All()
    {
        var min = Min;
        var max = Max;
        for (var z = min.z - 1; z <= max.z + 1; z++)
            for (var y = min.y - 1; y <= max.y + 1; y++)
                for (var x = min.x - 1; x <= max.x + 1; x++)
                    yield return new Coordinate(x, y, z);
    }

    public Grid Cycle()
    {
        var cells = (
            from coordinate in All().Distinct()
            let activeNeighbors = coordinate.Neighbors().Where(n => this[n] is Active).Count()
            let state = this[coordinate] switch
            {
                Active when activeNeighbors is 2 or 3 => Active,
                Inactive when activeNeighbors is 3 => Active,
                _ => Inactive
            }
            where state is Active
            select coordinate
        ).ToImmutableHashSet();

        return new Grid(cells);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();

        var min = Min;
        var max = Max;

        for (var z = min.z; z <= max.z; z++)
        {
            sb.AppendLine($"z = {z}");
            for (var y = min.y; y <= max.y; y++)
            {
                for (var x = min.x; x <= max.x; x++)
                {
                    sb.Append((char)this[new(x, y, z)]);
                }
                sb.AppendLine();
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }
}

enum State { Active = '#', Inactive = '.' };


public class Tests
{
    [Fact]
    public void Neighbors()
    {
        Assert.Equal(26, new Coordinate(0, 0, 0).Neighbors().Count());
    }

    [Fact]
    public void SingleActiveCellGrid()
    {
        var c = new Coordinate(0, 0, 0);
        var grid = new Grid(new[] { c }.ToImmutableHashSet());
        Assert.Equal(Active, grid[c]);
        foreach (var n in c.Neighbors())
            Assert.Equal(Inactive, grid[n]);
    }
    [Fact]
    public void Example()
    {
        var grid = Grid.FromLines(new[] { ".#.", "..#", "###" });
        for (var i = 0; i < 6; i++)
            grid = grid.Cycle();
        Assert.Equal(112, grid.ActiveCells.Count);
    }

}