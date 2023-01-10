namespace AdventOfCode.Year2018.Day23;

public class AoC201823
{
    static string[] input = Read.InputLines();

    static ImmutableArray<Octahedron> bots = (from line in input
                         let bot = Octahedron.TryParse(line)
                         where bot.HasValue select bot.Value).ToImmutableArray();
    static readonly Octahedron strongest = bots.MaxBy(b => b.range);
    public int Part1() => bots.Count(strongest.InRange);

    public object Part2()
    {
        var max = bots.Max(b => b.center.Max + b.range);
        long boxsize = (long)Pow(2, Ceiling(Log2(max)));
        var box = new Cube(new(-boxsize,-boxsize,-boxsize), new(boxsize, boxsize, boxsize));
        var priority = new BoxPriority(bots.Length, box.Size, 0);
        var pq = new PriorityQueue<Cube, BoxPriority>();
        pq.Enqueue(box, priority);

        while (pq.Count > 0)
        {
            box = pq.Dequeue();
            if (box.Size == 1)
            {
                return box.DistanceToOrigin;
            }


            var items = from octant in box.Octants()
                        let intersections = bots.Count(octant.IntersectsWith)
                        let prio = new BoxPriority(intersections, octant.Size, octant.DistanceToOrigin)
                        orderby prio
                        select (octant, prio);

            foreach (var (octant,prio) in items)
            {
                pq.Enqueue(octant, prio);
            }
        }
        return 0;
    }
}

readonly record struct Octahedron(Pos center, long range)
{
    static Regex regex = new(@"pos=<(?<X>[-\d]+),(?<Y>[-\d]+),(?<Z>[-\d]+)>, r=(?<r>[-\d]+)", RegexOptions.Compiled);
    public static Octahedron? TryParse(string s)
    {
        var match = regex.Match(s);
        return match.Success ? new Octahedron(new(
            long.Parse(match.Groups["X"].Value),
            long.Parse(match.Groups["Y"].Value),
            long.Parse(match.Groups["Z"].Value)),
            long.Parse(match.Groups["r"].Value)
            ) : null;
    }
    public bool InRange(Octahedron other) => other.center.Distance(center) <= range;
    public override string ToString() => $"Octahedron {center}/--[{range}]->";
}

readonly record struct Pos(long X, long Y, long Z)
{
    public static readonly Pos Origin = new(0, 0, 0);
    public long Distance(Pos other) => Abs(X - other.X) + Abs(Y - other.Y) + Abs(Z - other.Z);
    public long DistanceToOrigin => Abs(X) + Abs(Y) + Abs(Z);
    public long Max => (Abs(X), Abs(Y), Abs(Z)).Max();
    public override string ToString() => $"({X};{Y};{Z})";
}

// cube, defined by 2 juxtaposed points
readonly record struct Cube(Pos Lower, Pos Higher)
{
    public Pos center => new Pos((Higher.X + Lower.X) / 2, (Higher.Y + Lower.Y) / 2, (Higher.Z + Lower.Z) / 2);
    public long Size => Higher.X - Lower.X;

    public bool IntersectsWith(Octahedron octahedron)
    {
        var o = octahedron.center;
        var (l, h) = (Lower, Higher);
        var d = (Abs(o.X - l.X) + Abs(o.X - (h.X - 1)) - (h.X - l.X - 1)
               + Abs(o.Y - l.Y) + Abs(o.Y - (h.Y - 1)) - (h.Y - l.Y - 1)
               + Abs(o.Z - l.Z) + Abs(o.Z - (h.Z - 1)) - (h.Z - l.Z - 1)) / 2;
        return d <= octahedron.range;
    }
    IEnumerable<Pos> Corners()
    {
        yield return Lower;
        yield return new(Lower.X, Lower.Y, Higher.Z);
        yield return new(Lower.X, Higher.Y, Lower.Z);
        yield return new(Lower.X, Higher.Y, Higher.Z);
        yield return new(Higher.X, Higher.Y, Lower.Z);
        yield return new(Higher.X, Lower.Y, Lower.Z);
        yield return new(Higher.X, Lower.Y, Higher.Z);
        yield return Higher;
    }
    public IEnumerable<Cube> Octants()
    {
        var size = Size/2;
        var (l, h) = (Lower, Higher);
        return from x in Range(0, 2)
               from y in Range(0,2)
               from z in Range(0, 2)
               let lower = new Pos(l.X + size * x, l.Y + size * y, l.Z + size * z)
               let higher = new Pos(lower.X + size, lower.Y + size, lower.Z + size)
               select new Cube(lower, higher);

    }

    public long DistanceToOrigin => Corners().Min(c => c.DistanceToOrigin);
}

readonly record struct BoxPriority(int intersections, long size, long distance) : IComparable<BoxPriority>
{
    public int CompareTo(BoxPriority other)
    {
        var cmp = other.intersections.CompareTo(intersections);
        if (cmp == 0) cmp = other.size.CompareTo(size);
        if (cmp == 0) cmp = distance.CompareTo(other.distance);
        return cmp;
    }
}
