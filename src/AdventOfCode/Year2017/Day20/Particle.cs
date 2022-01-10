using static System.Math;

namespace AdventOfCode.Year2017.Day20;

struct Particle
{
    public readonly (double x, double y, double z) Position;
    public readonly (double x, double y, double z) Velocity;
    public readonly (double x, double y, double z) Acceleration;

    public Particle((double x, double y, double z) position, (double x, double y, double z) velocity, (double x, double y, double z) acceleration)
    {
        Position = position;
        Velocity = velocity;
        Acceleration = acceleration;
    }

    public Particle Tick()
    {
        var velocity = (x: Velocity.x + Acceleration.x, y: Velocity.y + Acceleration.y, z: Velocity.z + Acceleration.z);
        var position = (Position.x + velocity.x, Position.y + velocity.y, Position.z + velocity.z);
        return new Particle(position, velocity, Acceleration);
    }

    // pos(t) = p + vt + t(t+1)/2*a
    public (double x, double y, double z) GetPosition(double time)
    {
        return (
            calc(Position.x, Velocity.x, Acceleration.x, time),
            calc(Position.y, Velocity.y, Acceleration.y, time),
            calc(Position.z, Velocity.z, Acceleration.z, time)
            );
    }

    private double calc(double p, double v, double a, double time) => p + v * time + time * (time + 1) / 2 * a;

    public double Distance => Position.Distance();

    public static Particle Parse(string s)
    {
        var parts = (
            from part in s.Split(", ")
            let vector = part.Split("=")
            let name = vector[0]
            let values = vector[1].Trim(' ', '<', '>').Split(',').Select(int.Parse).ToArray()
            select (name: name, values: values)
        ).ToDictionary(x => x.name, x => (x.values[0], x.values[1], x.values[2]));
        return new Particle(parts["p"], parts["v"], parts["a"]);
    }
}

static class Ex
{
    public static double Distance(this (double x, double y, double z) pos) => Abs(pos.x) + Abs(pos.y) + Abs(pos.z);
    public static bool HasSingleItem<T>(this IEnumerable<T> input) => !input.Skip(1).Any();
}