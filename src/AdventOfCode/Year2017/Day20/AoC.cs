using static System.Math;

namespace AdventOfCode.Year2017.Day20;

public class AoC201720
{
    static Particle[] particles = Read.InputLines().Select(Particle.Parse).ToArray();
    static readonly double t = particles.Sum(p => Abs(p.Acceleration.x) + Abs(p.Acceleration.y) + Abs(p.Acceleration.z));
    public object Part1() => (
        from x in particles.Select((p, i) => (p: p, i: i))
        let position = x.p.GetPosition(t)
        let distance = position.Distance()
        select (index: x.i, particle: x.p, position: position, distance: distance)
        ).MinBy(x => x.distance).index;

    public object Part2() => Repeat(0, 100).Aggregate(particles, (set, i) => (
                from item in set
                select item.Tick() into tick
                group tick by tick.Position into g
                where g.HasSingleItem()
                select g.Single()
            ).ToArray()).Length;
}