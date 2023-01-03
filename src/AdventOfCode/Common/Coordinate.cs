﻿namespace AdventOfCode.Common;

readonly record struct Coordinate(int x, int y)
{
    public int ManhattanDistance(Coordinate o) => Abs(x - o.x) + Abs(y - o.y);
    public override string ToString() => $"({x},{y})";

    public static Coordinate operator +(Coordinate left, Coordinate right) => new(left.x + right.x, left.y + right.y);
    public static Coordinate operator -(Coordinate left, Coordinate right) => new(left.x - right.x, left.y - right.y);
}