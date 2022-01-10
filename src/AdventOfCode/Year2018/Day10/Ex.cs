namespace AdventOfCode.Year2018.Day10;

static class Ex
{
    public static Grid FindGridWithLowestHeight(this Grid grid) => grid.KeepMoving().Where(g => g.Height < g.Move(1).Height).First();
    static IEnumerable<Grid> KeepMoving(this Grid grid)
    {
        while (true)
        {
            grid = grid.Move(1);
            yield return grid;
        }
    }
}