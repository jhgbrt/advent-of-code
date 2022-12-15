namespace AdventOfCode.Common;

readonly record struct Point(int x, int y)
{
    public int Distance(Point other) => Math.Abs(x - other.x) + Math.Abs(y - other.y);
    public override string ToString() => $"({x},{y})";
}
