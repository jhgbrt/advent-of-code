namespace AdventOfCode.Common;

readonly record struct Coordinate(int x, int y)
{
    public static Coordinate Origin = new(0, 0);
    public int ManhattanDistance(Coordinate o) => Abs(x - o.x) + Abs(y - o.y);
    public override string ToString() => $"({x},{y})";

    public static Slope operator -(Coordinate left, Coordinate right) => new(left.x - right.x, left.y - right.y);
    public double Angle(Coordinate other) => -Atan2(x - other.x, y - other.y);
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
