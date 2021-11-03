using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;
using static System.Math;

class Program
{
    static void Main(string[] args)
    {
        var particles = File.ReadLines("input.txt").Select(Particle.Parse).ToArray();
        var t =  particles.Sum(p => Abs(p.Acceleration.x) + Abs(p.Acceleration.y) + Abs(p.Acceleration.z));

        Run(() => (
            from x in particles.Select((p, i) => (p: p, i: i))
            let position = x.p.GetPosition(t)
            let distance = position.Distance()
            select (index: x.i, particle: x.p, position: position, distance: distance)
        ).MinBy(x => x.distance).index);

        Run(() => Repeat(0, 100).Aggregate(particles, (set, i) => (
                from item in set
                select item.Tick() into tick
                group tick by tick.Position into g
                where g.HasSingleItem()
                select g.Single()
            ).ToArray()).Length
        );
    }
    
    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}