using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AdventOfCode.Year2019.Day12;

public class AoC201912
{
    public AoC201912() : this(Read.InputLines()) { }
    public AoC201912(string[] input)
    {
        this.input = input;
    }

    internal string[] input;
    ImmutableArray<Moon> GetMoons() => (
        from line in input
        let position = Regexes.Coordinate().As<Coordinate3D>(line)
        select new Moon(position, Velocity.Zero)
        ).ToImmutableArray();

    public object Part1(int steps) => GetMoons().Steps(steps).Sum(m => m.PotentialEnergy * m.KineticEnergy);
    public object Part1() => Part1(1000);
    public object Part2() => GetMoons().FindSteps();

    


}
static class Ex{
    internal static BigInteger FindSteps(this ImmutableArray<Moon> moons)
    {
        var stepsx = Ex.FindSteps(moons, m => (m.position.x, m.velocity.dx));
        var stepsy = Ex.FindSteps(moons, m => (m.position.y, m.velocity.dy));
        var stepsz = Ex.FindSteps(moons, m => (m.position.z, m.velocity.dz));
        return LeastCommonMultiplier(stepsx, stepsy, stepsz);
    }
    internal static long FindSteps(this ImmutableArray<Moon> moons, Func<Moon, (int, int)> f)
    {
        long steps = 0;
        var initial = moons.Select(f).ToArray();
        do
        {
            steps++;
            moons = Step(moons);
        } while (!moons.Select(f).SequenceEqual(initial));
        return steps;
    }
    internal static ImmutableArray<Moon> Steps(this ImmutableArray<Moon> moons, int steps)
        => Range(0, steps).Aggregate(moons, (moons, _) => Step(moons));

    private static ImmutableArray<Moon> Step(ImmutableArray<Moon> moons)
        => moons
            .Select(moon => moon.AdjustVelocity(moons.Where(m => m != moon)).Move())
            .ToImmutableArray();

}

readonly record struct Moon(Coordinate3D position, Velocity velocity)
{
    public Moon AdjustVelocity(IEnumerable<Moon> other)
    {
        var (position, velocity) = this;
        return this with { velocity = other.Aggregate(velocity, (v, other) => v.ApplyGravity(other.position - position)) };
    }

    public Moon Move() => this with { position = position + velocity };
    public int PotentialEnergy => Abs(position.x) + Abs(position.y) + Abs(position.z);
    public int KineticEnergy => Abs(velocity.dx) + Abs(velocity.dy) + Abs(velocity.dz);
}
readonly record struct Velocity(int dx, int dy, int dz)
{
    public readonly static Velocity Zero = new(0, 0, 0);
    public Velocity ApplyGravity(Slope3D delta) => this with
    {
        dx = delta.dx switch { > 0 => dx + 1, < 0 => dx - 1, 0 => dx },
        dy = delta.dy switch { > 0 => dy + 1, < 0 => dy - 1, 0 => dy },
        dz = delta.dz switch { > 0 => dz + 1, < 0 => dz - 1, 0 => dz },
    };
}

readonly record struct Coordinate3D(int x, int y, int z)
{
    public readonly static Coordinate3D Origin = new(0, 0, 0);
    public override string ToString() => $"({x},{y};{z})";
    public static Coordinate3D operator +(Coordinate3D position, Velocity v) => position with { x = position.x + v.dx, y = position.y + v.dy, z = position.z + v.dz };
    public static Slope3D operator -(Coordinate3D left, Coordinate3D right)
        => new(left.x - right.x, left.y - right.y, left.z - right.z);
}
readonly record struct Slope3D(int dx, int dy, int dz)
{
    public override string ToString() => $"({dx},{dy})";
}

static partial class Regexes
{
    [GeneratedRegex("<x=(?<x>[^,]*), y=(?<y>[^,]*), z=(?<z>[^>]*)>")]
    internal static partial Regex Coordinate();
}

public class Tests
{

    [Fact]
    public void Test1()
    {
        var sut = new AoC201912(Read.SampleLines(1));
        Assert.Equal(179, sut.Part1(10));
    }

    [Fact]
    public void Test2()
    {
        var sut = new AoC201912(Read.SampleLines(2));
        Assert.Equal(1940, sut.Part1(100));
    }

    [Fact]
    public void Test3()
    {
        var sut = new AoC201912(Read.SampleLines(2));
        Assert.Equal(new BigInteger(4686774924), sut.Part2());
    }

}