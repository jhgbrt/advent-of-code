namespace AdventOfCode.Year2018.Day18;

class Grid
{
    public int Lumber => _grid.OfType<Acre>().Aggregate(0, (c, a) => c + (a == Acre.Lumber ? 1 : 0));
    public int Tree => _grid.OfType<Acre>().Aggregate(0, (c, a) => c + (a == Acre.Tree ? 1 : 0));
    public int Value => Tree * Lumber;

    Acre[,] _grid;
    public Grid(string[] input)
    {
        _grid = new Acre[input.Length, input.Max(c => c.Length)];
        for (var y = 0; y < input.Length; y++)
            for (var x = 0; x < input[y].Length; x++)
                _grid[y, x] = (Acre)input[y][x];
    }

    private Grid(Acre[,] grid)
    {
        _grid = grid;
    }

    public Grid Step(int steps)
    {
        var result = this;
        for (int i = 0; i < steps; i++)
        {
            result = result.Step();
        }
        return result;
    }

    public Grid Step()
    {
        var grid = new Acre[_grid.GetLength(0), _grid.GetLength(1)];
        Array.Copy(_grid, grid, _grid.Length);


        var q = from c in Coordinates()
                let surroundings = _grid.Surroundings(c.x, c.y).ToArray()
                select (c, newchar: Morph(_grid[c.y, c.x], surroundings));

        foreach (var item in q)
            grid[item.c.y, item.c.x] = item.newchar;

        return new Grid(grid);
    }
    public static Acre Morph(Acre original, IEnumerable<Acre> surroundings)
    {
        switch (original)
        {
            case Acre.Open when surroundings.Count(c => c == Acre.Tree) >= 3:
                return Acre.Tree;
            case Acre.Tree when surroundings.Count(c => c == Acre.Lumber) >= 3:
                return Acre.Lumber;
            case Acre.Lumber when surroundings.Any(c => c == Acre.Tree) && surroundings.Any(c => c == Acre.Lumber):
                return Acre.Lumber;
            case Acre.Lumber:
                return Acre.Open;
        }
        return original;
    }

    IEnumerable<(int x, int y)> Coordinates()
    {
        for (var y = 0; y < _grid.GetLength(0); y++)
            for (var x = 0; x < _grid.GetLength(1); x++)
                yield return (x, y);

    }


    public override string ToString()
    {
        var sb = new StringBuilder();
        for (var y = 0; y < _grid.GetLength(0); y++)
        {
            for (var x = 0; x < _grid.GetLength(1); x++)
            {
                sb.Append((char)_grid[y, x]);
            }
            if (y < _grid.GetLength(0) - 1) sb.AppendLine();
        }
        return sb.ToString();
    }
}