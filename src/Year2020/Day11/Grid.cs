namespace AdventOfCode.Year2020.Day11;

class Grid
{
    char[,] _grid;
    public Grid(char[,] grid) => _grid = grid;
    char this[int x, int y] { get => _grid[y, x]; set => _grid[y, x] = value; }
    int Height => _grid.GetLength(0);
    int Width => _grid.GetLength(1);

    IEnumerable<(int x, int y, char c)> All()
    {
        for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
                yield return (x, y, this[x, y]);
    }
    public static Grid FromFile(string filename) => Parse(Read.Lines(typeof(AoCImpl), filename).ToArray());
    public static Grid FromString(string grid) => Parse(ReadLines(grid).ToArray());
    private static IEnumerable<string> ReadLines(string input)
    {
        using var sr = new StringReader(input);
        while (sr.Peek() >= 0) yield return sr.ReadLine()!;
    }

    public static Grid Parse(string[] lines)
    {
        var grid = new Grid(new char[lines.Length, lines[0].Length]);
        for (var y = 0; y < lines.Length; y++)
            for (var x = 0; x < lines[0].Length; x++)
                grid[x, y] = lines[y][x];
        return grid;
    }
    public long Handle(Func<Grid, int, int, char> rule)
    {
        var next = this;
        string s, nexts;
        do
        {
            s = next.ToString();
            next = next.Transform(rule);
            nexts = next.ToString();
        } while (s != nexts);
        return next.Count('#');
    }
    public Grid Transform(Func<Grid, int, int, char> rule)
    {
        var next = new Grid(new char[Height, Width]);
        foreach (var cell in All())
            next[cell.x, cell.y] = rule(this, cell.x, cell.y);
        return next;
    }
    public static char Rule1(Grid grid, int x, int y) => grid[x, y] switch
    {
        '.' => '.',
        'L' when grid.LookAround(x, y, _ => true).Count(c => c == '#') == 0 => '#',
        '#' when grid.LookAround(x, y, _ => true).Count(c => c == '#') >= 4 => 'L',
        char c => c
    };

    public static char Rule2(Grid grid, int x, int y) => grid[x, y] switch
    {
        '.' => '.',
        'L' when grid.LookAround(x, y, c => c != '.').Count(c => c == '#') == 0 => '#',
        '#' when grid.LookAround(x, y, c => c != '.').Count(c => c == '#') >= 5 => 'L',
        char c => c
    };

    private IEnumerable<char> LookAround(int x, int y, Func<char, bool> predicate)
    {
        yield return N(x, y).FirstOrDefault(predicate);
        yield return NE(x, y).FirstOrDefault(predicate);
        yield return E(x, y).FirstOrDefault(predicate);
        yield return SE(x, y).FirstOrDefault(predicate);
        yield return S(x, y).FirstOrDefault(predicate);
        yield return SW(x, y).FirstOrDefault(predicate);
        yield return W(x, y).FirstOrDefault(predicate);
        yield return NW(x, y).FirstOrDefault(predicate);
    }

    IEnumerable<char> NW(int x, int y)
    {
        while (--x >= 0 && --y >= 0) yield return this[x, y];
    }
    IEnumerable<char> N(int x, int y)
    {
        while (--y >= 0) yield return this[x, y];
    }
    IEnumerable<char> NE(int x, int y)
    {
        while (--y >= 0 && ++x < Width) yield return this[x, y];
    }
    IEnumerable<char> E(int x, int y)
    {
        while (++x < Width) yield return this[x, y];
    }
    IEnumerable<char> SE(int x, int y)
    {
        while (++y < Height && ++x < Width) yield return this[x, y];
    }
    IEnumerable<char> S(int x, int y)
    {
        while (++y < Height) yield return this[x, y];
    }
    IEnumerable<char> SW(int x, int y)
    {
        while (++y < Height && --x >= 0) yield return this[x, y];
    }
    IEnumerable<char> W(int x, int y)
    {
        while (--x >= 0) yield return this[x, y];
    }
    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++) sb.Append(this[x, y]);
            sb.AppendLine();
        }
        return sb.ToString();
    }
    public int Count(char c) => All().Count(x => x.c == c);
}