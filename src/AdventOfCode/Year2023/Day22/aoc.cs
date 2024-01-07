using Microsoft.CodeAnalysis;

namespace AdventOfCode.Year2023.Day22;
public class AoC202322
{
    public AoC202322() : this(Read.InputLines(), Console.Out) { }
    readonly TextWriter writer;
    readonly ImmutableArray<Brick> bricks;
    internal readonly ILookup<Brick, Brick> bricksSupporting;
    internal readonly ILookup<Brick, Brick> bricksSupportedBy;
    internal IEnumerable<Brick> Items => bricks;
    public AoC202322(string[] input, TextWriter writer)
    {
        bricks = DropAll(
            from brick in input.Select((s, i) => Regexes.MyRegex().As<Item>(s).ToBrick((char)(i + 'A')))
            orderby brick.Elevation
            select brick
            ).ToImmutableArray();
        
        bricksSupporting = (
            from b in bricks
            from s in bricks
            where b != s && s.IsSupportedBy(b)
            select (s, b)
            ).ToLookup(g => g.s, g => g.b);

        bricksSupportedBy = (
            from b in bricks
            from s in bricks
            where b != s && s.Supports(b)
            select (s, b)
            ).ToLookup(g => g.s, g => g.b);
        this.writer = writer;
    }

    public int Part1() => (
        from brick in bricks
        let supported = bricksSupportedBy[brick]
        where !supported.Any() || supported.All(s => bricksSupporting[s].Count() > 1)
        select brick
        ).Count();

    public int Part2() => (
        from brick in bricks
        let supported = bricksSupportedBy[brick]
        where supported.Any(s => bricksSupporting[s].Count() == 1)
        select CountFallingBricks(brick)
        ).Sum();

    private int CountFallingBricks(Brick brick)
    {
        HashSet<Brick> falls = [];
        Queue<Brick> q = new Queue<Brick>();
        q.Enqueue(brick);
        while (q.Any())
        {
            var b = q.Dequeue();
            falls.Add(b);
            foreach (var s in bricksSupportedBy[b].Where(s => !bricksSupporting[s].Except(falls).Any()))
            {               
                q.Enqueue(s);
            }
        }
        return falls.Count - 1;
    }
    ImmutableArray<Brick> DropAll(IEnumerable<Brick> bricks)
    {
        var dropped = new List<Brick>();
        foreach (var item in bricks)
        {
            dropped.Add(Drop(item, dropped));
        }
        return dropped.ToImmutableArray();
    }

    Brick Drop(Brick brick, IEnumerable<Brick> dropped)
    {
        var z = dropped.Any() ? dropped.Max(b => b.c2.z) : 0;

        brick = brick.SetElevation(z + 1);
        while (brick.Elevation > 1)
        {
            brick = brick.SetElevation(brick.Elevation - 1);
            if (dropped.Any(brick.Intersects))
            {
                brick = brick.SetElevation(brick.Elevation + 1);
                break;
            }
        }

        return brick;
    }

  
}


readonly record struct Item(int x1, int y1, int z1, int x2, int y2, int z2)
{
    public Brick ToBrick(int id) => new(id, new(Min(x1, x2), Min(y1, y2), Min(z1, z2)), new(Max(x1, x2), Max(y1, y2), Max(z1, z2)));
}
readonly record struct Brick(int id, Point c1, Point c2)
{
    public int Elevation => c1.z;

    public Brick SetElevation(int z)
    {
        var length = c2.z - c1.z;
        return this with { c1 = c1 with { z = z }, c2 = c2 with { z = z + length } };
    }

    public bool Intersects(Brick brick)
    => c1.x <= brick.c2.x && c2.x >= brick.c1.x &&
       c1.y <= brick.c2.y && c2.y >= brick.c1.y && 
       c1.z <= brick.c2.z && c2.z >= brick.c1.z;

    public bool IsSupportedBy(Brick brick)
    => (this with { c1 = c1.Down(), c2 = c2.Down() }).Intersects(brick);

    public bool Supports(Brick brick)
    => (this with { c1 = c1.Up(), c2 = c2.Up() }).Intersects(brick);

    public override string ToString()
    {
        return $"{id} {c1}~{c2}";
    }

}

readonly record struct Point(int x, int y, int z)
{
    public override string ToString() => $"({x},{y};{z})";
    public Point Up() => this with { z = z + 1 };
    public Point Down() => this with { z = z - 1 };
}

static partial class Regexes
{
    [GeneratedRegex(@"^(?<x1>\d+),(?<y1>\d+),(?<z1>\d+)~(?<x2>\d+),(?<y2>\d+),(?<z2>\d+)$")]
    public static partial Regex MyRegex();
}

public class AoC202322Tests
{
    private readonly AoC202322 sut;
    private readonly ITestOutputHelper output;
    public AoC202322Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        this.output = output;
        sut = new AoC202322(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(7, sut.Items.Count());
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(5, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(7, sut.Part2());
    }


}

