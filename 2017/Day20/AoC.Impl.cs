using static System.Linq.Enumerable;
using static System.Math;

namespace AdventOfCode.Year2017.Day20;

partial class AoC
{
    static Particle[] particles = File.ReadLines("input.txt").Select(Particle.Parse).ToArray();
    static readonly double t = particles.Sum(p => Abs(p.Acceleration.x) + Abs(p.Acceleration.y) + Abs(p.Acceleration.z));
    internal static Result Part1() => Run(() => (
        from x in particles.Select((p, i) => (p: p, i: i))
        let position = x.p.GetPosition(t)
        let distance = position.Distance()
        select (index: x.i, particle: x.p, position: position, distance: distance)
        ).MinBy(x => x.distance).index);

    internal static Result Part2() => Run(() => Repeat(0, 100).Aggregate(particles, (set, i) => (
                from item in set
                select item.Tick() into tick
                group tick by tick.Position into g
                where g.HasSingleItem()
                select g.Single()
            ).ToArray()).Length
        );
}