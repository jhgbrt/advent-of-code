namespace AdventOfCode.Common;

readonly record struct Point(int x, int y)
{
    public int ManhattanDistance(Point o) => Math.Abs(x - o.x) + Math.Abs(y - o.y);
    public override string ToString() => $"({x},{y})";
}
