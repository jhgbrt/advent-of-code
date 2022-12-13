namespace AdventOfCode.Common;

readonly record struct Point(int x, int y)
{
    public override string ToString() => $"({x},{y})";
}
