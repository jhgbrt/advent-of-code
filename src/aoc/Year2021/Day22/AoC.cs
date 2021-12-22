using static System.Math;
namespace AdventOfCode.Year2021.Day22;

public class AoC202122
{
    static string[] input = Read.InputLines();

    static ImmutableArray<Cuboid> cuboids = (from line in input
                                             let v = Cuboid.Parse(line)
                                             where v.HasValue
                                             select v.Value).ToImmutableArray();
    public object Part1() => (
        from instruction in cuboids
        let c = instruction.cube
        where c.p1 > new P(-51, -51, -51) && c.p2 < new P(51, 51, 51)
        from p in c.Points()
        select (instruction.@on, p)
        ).Aggregate(ImmutableHashSet<P>.Empty, (set, p) => p.on ? set.Add(p.p) : set.Remove(p.p)).Count();

    public object Part2()
    {
        var mm = cuboids
            .Select(c => c.cube.p2)
            .Aggregate((
                max: new P(int.MinValue, int.MinValue, int.MinValue),
                min: new P(int.MaxValue, int.MaxValue, int.MaxValue)
             ),
             (m, c) => (
                max: new P(Max(m.max.x, c.x), Max(m.max.y, c.y), Max(m.max.z, c.z)),
                min: new P(Min(m.min.x, c.x), Min(m.min.y, c.y), Min(m.min.z, c.z))
             )
            );
        var start = new Cuboid(false, new Cube(mm.min, mm.max));

        

        Console.WriteLine(start);
        return 0;
    }
}


record struct Cuboid(bool on, Cube cube)
{
    static Regex regex = new Regex(@"(?<switch>on|off) x=(?<x1>[-\d]+)..(?<x2>[-\d]+),y=(?<y1>[-\d]+)..(?<y2>[-\d]+),z=(?<z1>[-\d]+)..(?<z2>[-\d]+)", RegexOptions.Compiled);
    public static Cuboid? Parse(string s)
    {
        var m = regex.Match(s);
        if (m.Success)
        {
            var p1 = new P(m.GetInt32("x1"), m.GetInt32("y1"), m.GetInt32("z1"));
            var p2 = new P(m.GetInt32("x2"), m.GetInt32("y2"), m.GetInt32("z2"));
            var c = new Cuboid(
              m.Groups["switch"].Value == "on",
              new Cube(p1 < p2 ? p1 : p2, p1 < p2 ? p2 : p1)
            );
            return c;
        }
        else
        {
            return null;
        }
    }
  
}

record struct Cube(P p1, P p2)
{
    public long Volume => (p2.x - p1.x) * (p2.y - p1.y) * (p2.z - p1.z);
    public Cube? Intersect(Cube other)
    {
        var i1 = new P(Max(p1.x, other.p1.x), Max(p1.y, other.p1.y), Max(p1.z, other.p1.z));
        var i2 = new P(Min(p2.x, other.p2.x), Min(p2.y, other.p2.y), Min(p2.z, other.p2.z));
        if (i1 < i2) return new Cube(i1, i2);
        else return null;
    }
    public IEnumerable<P> Points()
    {
        var p1 = this.p1;
        var p2 = this.p2;
        return from x in Range(p1.x, p2.x - p1.x + 1)
               from y in Range(p1.y, p2.y - p1.y + 1)
               from z in Range(p1.z, p2.z - p1.z + 1)
               select new P(x, y, z);
    }
}

record struct P(int x, int y, int z)
{
    public static bool operator <(P left, P right) => left.x < right.x && left.y < right.y && left.z < right.z;
    public static bool operator >(P left, P right) => left.x > right.x && left.y > right.y && left.z > right.z;
    public static bool operator <=(P left, P right) => left.x <= right.x && left.y <= right.y && left.z <= right.z;
    public static bool operator >=(P left, P right) => left.x >= right.x && left.y >= right.y && left.z >= right.z;
}

static class Ext
{
    public static int GetInt32(this Match m, string name) => int.Parse(m.Groups[name].Value);
}

public class Tests
{
    [Fact]
    public void IntersectionTest()
    {
        var cube1 = new Cube(new P(0, 0, 0), new P(3, 3, 3));

        Assert.Equal(new Cube(new P(1, 1, 1), new P(2, 2, 2)), cube1.Intersect(new Cube(new P(1, 1, 1), new P(2, 2, 2))));
        Assert.Equal(new Cube(new P(0, 0, 0), new P(2, 2, 2)), cube1.Intersect(new Cube(new P(-1, -1, -1), new P(2, 2, 2))));
        Assert.Equal(new Cube(new P(0, 0, 0), new P(1, 1, 1)), cube1.Intersect(new Cube(new P(-1, -1, -1), new P(1, 1, 1))));
        Assert.Equal(new Cube(new P(0, 0, 0), new P(3, 3, 3)), cube1.Intersect(new Cube(new P(-1, -1, -1), new P(4, 4, 4))));
        Assert.Equal(new Cube(new P(1, 1, 1), new P(3, 3, 3)), cube1.Intersect(new Cube(new P(1, 1, 1), new P(4, 4, 4))));

    }
}