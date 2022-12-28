namespace AdventOfCode.Year2018.Day17;

static class Ex
{
    public static (int x, int y) Up(this (int x, int y) c) => (c.x, c.y - 1);
    public static (int x, int y) Down(this (int x, int y) c) => (c.x, c.y + 1);
    public static (int x, int y) Left(this (int x, int y) c) => (c.x - 1, c.y);
    public static (int x, int y) Right(this (int x, int y) c) => (c.x + 1, c.y);

    public static IEnumerable<(int x, int y)> MoveLeft(this (int x, int y) pos, Func<(int x, int y), bool> predicate)
    {
        while (true)
        {
            yield return pos = pos.Left();
            if (!predicate(pos))
                yield break;
        }
    }
    public static IEnumerable<(int x, int y)> MoveRight(this (int x, int y) pos, Func<(int x, int y), bool> predicate)
    {
        while (true)
        {
            yield return pos = pos.Right();
            if (!predicate(pos))
                yield break;
        }
    }
    public static IEnumerable<(int x, int y)> MoveDown(this (int x, int y) pos, Func<(int x, int y), bool> predicate)
    {
        while (true)
        {
            yield return pos = pos.Down();
            if (!predicate(pos))
                yield break;
        }
    }
    public static IEnumerable<IEnumerable<T>> Rows<T>(this T[,] array)
    {
        for (int r = array.GetLowerBound(0); r <= array.GetUpperBound(0); ++r)
            yield return row(array, r);
    }

    static IEnumerable<T> row<T>(T[,] array, int r)
    {
        for (int c = array.GetLowerBound(1); c <= array.GetUpperBound(1); ++c)
            yield return array[r, c];
    }
}