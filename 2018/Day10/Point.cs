using System.Text.RegularExpressions;

namespace AdventOfCode
{
    struct Point
    {
        static Regex regex = new Regex(
            @"position=\<\s*(?<x>-?\d+),\s*(?<y>-?\d+)\> velocity=\<\s*(?<dx>-?\d+),\s*(?<dy>-?\d+)\>",
            RegexOptions.Compiled);

        public readonly int X;
        public readonly int Y;
        public readonly int Vx;
        public readonly int Vy;

        public Point(int x, int y, int vx, int vy)
        {
            (X,Y, Vx, Vy) = (x,y,vx,vy);
        }
        public static Point Parse(string s)
        {
            var result = regex.Match(s);
            return new Point(
                new[] { "x", "y", "dx", "dy" }.Select(g => int.Parse(result.Groups[g].Value)).ToArray()
                );
        }
        private Point(int[] ints)
        {
            (X, Y, Vx, Vy) = (ints[0], ints[1], ints[2], ints[3]);
        }
        public Point Move(int seconds) => new Point(X + seconds * Vx, Y + seconds * Vy, Vx, Vy);

        public override bool Equals(object obj) => Equals((Point)obj);
        public bool Equals(Point other) => (X,Y,Vx,Vy) == (other.X, other.Y, other.Vx, other.Vy);
        public override int GetHashCode() => (X, Y, Vx, Vy).GetHashCode();
        public static bool operator ==(Point left, Point right) => left.Equals(right);
        public static bool operator !=(Point left, Point right) => !left.Equals(right);
        public override string ToString() => (X, Y, Vx, Vy).ToString();
    }
}
