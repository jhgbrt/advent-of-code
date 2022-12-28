using static System.Math;

var particles = File.ReadAllLines("input.txt").Select(Particle.Parse).ToArray();
var t = particles.Sum(p => Abs(p.Acceleration.x) + Abs(p.Acceleration.y) + Abs(p.Acceleration.z));
var sw = Stopwatch.StartNew();
var part1 = (
    from x in particles.Select((p, i) => (p: p, i: i)) let position = x.p.GetPosition(t) let distance = position.Distance() select (index: x.i, particle: x.p, position: position, distance: distance)).MinBy(x => x.distance).index;
var part2 = Repeat(0, 100).Aggregate(particles, (set, i) => (
    from item in set
    select item.Tick() into tick
    group tick by tick.Position into g
    where g.HasSingleItem()
    select g.Single()).ToArray()).Length;
Console.WriteLine((part1, part2, sw.Elapsed));