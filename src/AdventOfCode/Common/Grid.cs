namespace AdventOfCode.Common;

class Grid
{
    readonly string[] input;
    readonly Point origin = new(0, 0);
    readonly Point length;

    public int Height => length.y;
    public int Width => length.x;
    public int Circumference => 2 * (Height - 1) + 2 * (Width - 1);
    public Grid(string[] input)
    {
        this.input = input;
        length = new(input[0].Length, input.Length);
    }
    public int this[Point p] => input[p.y][p.x];

    public IEnumerable<Point> Points() =>
        from y in Range(origin.y, length.y)
        from x in Range(origin.x, length.x)
        select new Point(x, y);

    public IEnumerable<Point> InteriorPoints() =>
        from y in Range(origin.y + 1, length.y - 2)
        from x in Range(origin.x + 1, length.x - 2)
        select new Point(x, y);

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y < length.y; y++)
        {
            for (int x = 0; x < length.x; x++) sb.Append(this[new(x, y)]);
            sb.AppendLine();
        }
        return sb.ToString();
    }
}
