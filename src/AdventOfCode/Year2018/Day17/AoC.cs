namespace AdventOfCode.Year2018.Day17;

public class AoC201817
{
    static string[] input = Read.InputLines();

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);
    public static int Part1(string[] input)
    {
        var grid = Grid.Parse(input);
        grid.Simulate();
        return grid.NofWaterReachableTiles;
    }

    public static int Part2(string[] input)
    {
        var grid = Grid.Parse(input);
        grid.Simulate();
        return grid.NofWaterTiles;
    }

}

class Grid
{
    private char[,] grid;

    public int MaxY;
    public int MinX;
    public int MaxX;
    private (int x, int y) Source = (500, 0);

    public Grid((int x, int y)[] clay)
    {
        MaxY = clay.Max(x => x.y);
        MinX = clay.Min(x => x.x - 1);
        MaxX = clay.Max(x => x.x + 1);
        grid = new char[MaxY + 1, MaxX - MinX + 1];
        for (int y = 0; y < MaxY + 1; y++)
            for (int x = MinX; x <= MaxX; x++)
                this[(x, y)] = '.';
        foreach (var item in clay)
            this[item] = '#';
        this[Source] = '+';
    }

    public int NofWaterReachableTiles
        => grid.Rows().SkipWhile(r => r.All(c => c != '#')).SelectMany(c => c).Count(c => c == '|' || c == '~');

    public int NofWaterTiles
        => grid.Rows().SkipWhile(r => r.All(c => c != '#')).SelectMany(c => c).Count(c => c == '~');

    public void Simulate()
    {
        DoDrop(Source);
    }
    void DoDrop((int x, int y) position)
    {
        position = GoDown(position);
        if (position.y >= MaxY)
        {
            return;
        }

        while (true)
        {
            var left = CanMoveLeft(position) ? position.MoveLeft(CanMoveLeft).Last() : position;
            var right = CanMoveRight(position) ? position.MoveRight(CanMoveRight).Last() : position;

            if (CanMoveDown(left) || CanMoveDown(right))
            {
                for (var p = left; p.x <= right.x; p = p.Right())
                    this[p] = '|';

                if (CanMoveDown(left) && this[left.Down()] != '|')
                    DoDrop(left);

                if (CanMoveDown(right) && this[right.Down()] != '|')
                    DoDrop(right);

                return;
            }

            for (var p = left; p.x <= right.x; p = p.Right())
                this[p] = '~';

            position = position.Up();

        }

    }

    private (int x, int y) GoDown((int x, int y) position)
    {
        if (CanMoveDown(position))
        {
            foreach (var p in position.MoveDown(p => p.y < MaxY && CanMoveDown(p)))
            {
                this[p] = '|';
                position = p;
            }
        }

        return position;
    }

    private (int x, int y) FillUpward((int x, int y) position)
    {
        do
        {
            this[position] = '~';
            if (CanMoveLeft(position)) foreach (var p in position.MoveLeft(CanMoveLeft)) this[p] = '~';
            if (CanMoveRight(position)) foreach (var p in position.MoveRight(CanMoveRight)) this[p] = '~';
            position = position.Up();
        } while (CanSettle(position));
        return position;
    }

    private (int x, int y) GoRight((int x, int y) position)
    {

        if (CanMoveRight(position))
        {
            foreach (var p in position.MoveRight(CanMoveRight))
            {
                this[p] = '|';
                position = p;
            }
        }

        if (CanMoveDown(position) && this[position.Down()] != '|')
            DoDrop(position);

        return position;
    }

    private (int x, int y) GoLeft((int x, int y) position)
    {
        if (CanMoveLeft(position))
        {
            foreach (var p in position.MoveLeft(CanMoveLeft))
            {
                this[p] = '|';
                position = p;
            }
        }

        if (CanMoveDown(position) && this[position.Down()] != '|')
            DoDrop(position);

        return position;
    }

    bool CanSettle((int x, int y) pos)
    {
        if (this[pos] == '~' || this[pos] == '#') return false;
        if (CanMoveDown(pos)) return false;
        if (CanMoveLeft(pos))
        {
            var left = pos.MoveLeft(CanMoveLeft).Last();
            if (left.x <= MinX) return false;
            if (CanMoveDown(left)) return false;
        }
        if (CanMoveRight(pos))
        {
            var right = pos.MoveRight(CanMoveRight).Last();
            if (right.x >= MaxX) return false;
            if (CanMoveDown(right)) return false;
        }
        return true;
    }


    bool CanMoveDown((int x, int y) pos) => this[pos.Down()] == '|' || this[pos.Down()] == '.';
    bool CanMoveLeft((int x, int y) pos) => !CanMoveDown(pos) && (this[pos.Left()] == '|' || this[pos.Left()] == '.');
    bool CanMoveRight((int x, int y) pos) => !CanMoveDown(pos) && (this[pos.Right()] == '|' || this[pos.Right()] == '.');

    char this[(int x, int y) c]
    {
        get
        {
            var x = c.x - MinX;
            var y = c.y;
            if (x < 0 || x >= grid.GetLength(1) || y < 0 || y >= grid.GetLength(0)) return '.';
            return grid[y, x];
        }
        set { grid[c.y, c.x - MinX] = value; }
    }

    public static Grid Parse(IEnumerable<string> input)
    {
        var clay = input.SelectMany(line => ParseLine(line)).ToArray();
        return new Grid(clay);
    }
    static Regex regex = new Regex(@"(?<v1>\w)=(?<n1>\d+), (?<v2>\w)=(?<n2>\d+)\.\.(?<n3>\d+)");
    public static IEnumerable<(int x, int y)> ParseLine(string line)
    {
        var match = regex.Match(line);
        var v1 = match.Groups["v1"].Value;
        var n1 = int.Parse(match.Groups["n1"].Value);
        var n2 = int.Parse(match.Groups["n2"].Value);
        var n3 = int.Parse(match.Groups["n3"].Value);
        var range = Enumerable.Range(n2, n3 - n2 + 1);
        return v1 == "x" ? range.Select(y => (n1, y)) : range.Select(x => (x, n1));
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y <= MaxY; y++)
        {
            for (int x = MinX; x <= MaxX; x++)
            {
                sb.Append(this[(x, y)]);
            }
            if (y != MaxY)
                sb.AppendLine();
        }
        return sb.ToString();
    }
}


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