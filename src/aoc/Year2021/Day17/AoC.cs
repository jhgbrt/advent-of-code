namespace AdventOfCode.Year2021.Day17
{
    public class AoC202117
    {
        static string[] input = Read.InputLines();
        static Area area = new Area(new P(185, -74), new P(221, -74));

        public object Part1()
        {
            // target area: x=185..221, y=-122..-74

            var a = new Area(new P(20, -5), new P(30, -10));


            var maxY = int.MinValue;
            for (var dx = -100; dx<=100; dx++)
                for (var dy = -100; dy<=100; dy++)
                {
                    var v = new V(dx, dy);
                    var probe = new Probe(a, v, new P(0, 0), v, int.MinValue);
                    Console.WriteLine(probe);
                    while (!probe.Hit && !probe.Missed)
                    {
                        probe = probe.Step();
                    }
                    if (probe.Hit && probe.max > maxY)
                    {
                        maxY = probe.max;
                    }
                    Console.WriteLine(probe);

                }


            return maxY;

        }

        public object Part2() => -1;
    }

    record struct Probe(Area target, V v0, P p, V v, int max)
    {
        public bool Hit => p.IsIn(target);
        public bool Missed
        {
            get 
            {
                if (p.y < target.p2.y)
                    return true;
                if (v.dx == 0 && (p.x < target.p1.x || p.x > target.p2.x)) return true;
                return false;
            } 
        }
        public Probe Step() => this with { p = p.Next(v), v = v.Next(),  max = p.y > max ? p.y : max};
    }

    record struct Area(P p1, P p2);
    record struct P(int x, int y)
    {
        public P Next(V v) => this with { x = x + v.dx, y = y + v.dy };
        public bool IsIn(Area a) => x >= a.p1.x && x <= a.p1.x && y <= a.p2.y && y >= a.p2.y;
    }
    record struct V(int dx, int dy)
    {
        public V Next() => this with { dx = dx switch { > 0 => dx - 1, 0 => 0, < 0 => dx + 1 }, dy = dy - 1 };
    }
}
