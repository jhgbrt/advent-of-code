namespace AdventOfCode.Year2021.Day17;

public class AoC202117
{
    //static Area target  = new Area(new P(20, -5), new P(30, -10));
    static Area target = new Area(new P(185, -74), new P(221, -122));

    public object Part1() => GetHits(target).MaxBy(x => x.max).max;

    public object Part2() => GetHits(target).Count();

    static IEnumerable<Probe> GetHits(Area target)
        => from v in CandidateVelocities(target.bottomright)
           let p = DoProbe(target, v)
           where p.Hit
           select p;

    static Probe DoProbe(Area target, V v0)
    {
        var p = new Probe(target, new P(0, 0), v0, int.MinValue);
        while (!p.Hit && !p.Missed)
        {
            p = p.Step();
        }
        return p;
    }

    static IEnumerable<V> CandidateVelocities(P bottomright)
    {
        var max = (x:Math.Abs(bottomright.x), y:Math.Abs(bottomright.y));
        for (var dx = -Math.Abs(max.x); dx <= Math.Abs(max.x); dx++)
        for (var dy = -Math.Abs(max.y); dy <= Math.Abs(max.y); dy++)
            yield return new V(dx, dy);
    }
}

record struct Probe(Area target, P p, V v, int max)
{
    public bool Hit => p.IsIn(target);
    public bool Missed => p.y < target.bottomright.y || v.dx == 0 && (p.x < target.topleft.x || p.x > target.bottomright.x);
    public Probe Step() => this with { p = p.Next(v), v = v.Next(), max = p.y > max ? p.y : max };
    public override string ToString() => $"{(p.x, p.y)}, v = {(v.dx, v.dy)}, max = {max}, {(Hit ? "hit" : "")}/{(Missed ? "miss" : "")})";
}

record struct Area(P topleft, P bottomright);
record struct P(int x, int y)
{
    public P Next(V v) => this with { x = x + v.dx, y = y + v.dy };
    public bool IsIn(Area a) 
        => x >= a.topleft.x 
        && x <= a.bottomright.x 
        && y <= a.topleft.y 
        && y >= a.bottomright.y;
}
record struct V(int dx, int dy)
{
    public V Next() => this with { dx = dx switch { > 0 => dx - 1, 0 => 0, < 0 => dx + 1 }, dy = dy - 1 };
}

public class AoC202117Tests
{
    [Theory]
    [InlineData(0, 0, 1, 1, 1, 1)]
    [InlineData(0, 0, 1, -1, 1, -1)]
    [InlineData(0, 0, -1, 1, -1, 1)]
    [InlineData(0, 0, 5, -1, 5, -1)]
    [InlineData(10, 20, 5, -1, 15, 19)]
    public void PNext(int x, int y, int dx, int dy, int expectedx, int expectedy)
    {
        var p = new P(x, y);
        var v = new V(dx, dy);
        p = p.Next(v);
        Assert.Equal(new P(expectedx, expectedy), p);
    }
    [Theory]
    [InlineData(1, 1, 0, 0)]
    [InlineData(0, 0, 0, -1)]
    [InlineData(0, -1, 0, -2)]
    [InlineData(0, -10, 0, -11)]
    [InlineData(-1, 1, 0, 0)]
    [InlineData(-1, 0, 0, -1)]
    [InlineData(-10, 0, -9, -1)]
    public void VNext(int dx, int dy, int expectedx, int expectedy)
    {
        var v = new V(dx, dy);
        v = v.Next();
        Assert.Equal(new V(expectedx, expectedy), v);
    }

    [Fact]
    public void Area()
    {
        var a = new Area(new(20, -5), new P(30, -10));
        Assert.True(new P(20, -5).IsIn(a));
        Assert.True(new P(20, -10).IsIn(a));
        Assert.True(new P(30, -5).IsIn(a));
        Assert.True(new P(30, -10).IsIn(a));
    }

    [Fact]
    public void Probe()
    {
        var a = new Area(new(20, -5), new P(30, -10));
        var v0 = new V(7, 2);
        var p = new P(0, 0);

        var probe = new Probe(a, p, v0, 0);
        Assert.False(probe.Hit);
        Assert.False(probe.Missed);

        probe = probe.Step();
        Assert.Equal(new P(7, 2), probe.p);
        Assert.False(probe.Hit);
        Assert.False(probe.Missed);

        probe = probe.Step();
        Assert.Equal(new P(13, 3), probe.p);
        Assert.False(probe.Hit);
        Assert.False(probe.Missed);

        probe = probe.Step();
        Assert.Equal(new P(18, 3), probe.p);
        Assert.False(probe.Hit);
        Assert.False(probe.Missed);

        probe = probe.Step();
        Assert.Equal(new P(22, 2), probe.p);
        Assert.False(probe.Hit);
        Assert.False(probe.Missed);

        probe = probe.Step();
        Assert.Equal(new P(25, 0), probe.p);
        Assert.False(probe.Hit);
        Assert.False(probe.Missed);

        probe = probe.Step();
        Assert.Equal(new P(27, -3), probe.p);
        Assert.False(probe.Hit);
        Assert.False(probe.Missed);

        probe = probe.Step();
        Assert.Equal(new P(28, -7), probe.p);
        Assert.True(probe.Hit);
        Assert.False(probe.Missed);

    }
}