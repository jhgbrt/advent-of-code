namespace AdventOfCode.Year2018.Day18;

static class Ex
{

    public static IEnumerable<T> Surroundings<T>(this T[,] _grid, int x, int y)
    {
        var maxY = _grid.GetLength(0) - 1;
        var maxX = _grid.GetLength(1) - 1;

        if (y > 0 && x > 0)
            yield return _grid[y - 1, x - 1];
        if (x > 0)
            yield return _grid[y, x - 1];
        if (y < maxY && x > 0)
            yield return _grid[y + 1, x - 1];
        if (y < maxY)
            yield return _grid[y + 1, x];
        if (y < maxY && x < maxX)
            yield return _grid[y + 1, x + 1];
        if (x < maxX)
            yield return _grid[y, x + 1];
        if (y > 0 && x < maxX)
            yield return _grid[y - 1, x + 1];
        if (y > 0)
            yield return _grid[y - 1, x];
    }
}