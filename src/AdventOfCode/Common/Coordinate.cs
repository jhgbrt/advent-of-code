﻿namespace AdventOfCode;
readonly record struct Coordinate3D(int x, int y, int z)
{
    public static Coordinate3D Origin = new(0, 0, 0);
    public int ManhattanDistance(Coordinate3D o) => Abs(x - o.x) + Abs(y - o.y) + Abs(z - o.z);
    public override string ToString() => $"({x},{y};{z})";

    public static Slope3D operator -(Coordinate3D left, Coordinate3D right) 
        => new(left.x - right.x, left.y - right.y, left.z - right.z);
}
readonly record struct Slope3D(int dx, int dy, int dz)
{
    public Slope3D Gcd
    {
        get
        {
            var gcd = (int)BigInteger.GreatestCommonDivisor(dx, dy);
            gcd = (int)BigInteger.GreatestCommonDivisor(gcd, dz);
            return gcd > 0 ? new(dx / gcd, dy / gcd, dz / gcd) : this;
        }
    }

    public override string ToString() => $"({dx},{dy})";
}

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public int ManhattanDistance(Coordinate o) => Abs(x - o.x) + Abs(y - o.y);
    public override string ToString() => $"({x},{y})";

    public static Slope operator -(Coordinate left, Coordinate right) => new(left.x - right.x, left.y - right.y);
    public double Angle(Coordinate other) => -Atan2(x - other.x, y - other.y);
    public Coordinate N => new(x, y - 1);
    public Coordinate NE => new(x + 1, y - 1);
    public Coordinate E => new(x + 1, y);
    public Coordinate SE => new(x + 1, y + 1);
    public Coordinate S => new(x, y + 1);
    public Coordinate SW => new(x - 1, y + 1);
    public Coordinate W => new(x - 1, y);
    public Coordinate NW => new(x - 1, y - 1);
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
}

readonly record struct Slope(int dx, int dy)
{
    public Slope Gcd
    {
        get
        {
            var gcd = (int)BigInteger.GreatestCommonDivisor(dx, dy);
            return gcd > 0 ? new(dx / gcd, dy / gcd) : this;
        }
    }

    public override string ToString() => $"({dx},{dy})";
}
