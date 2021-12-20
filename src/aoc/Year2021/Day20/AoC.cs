namespace AdventOfCode.Year2021.Day20;

public class AoC202120
{
    static string[] input = Read.InputLines();
    static string algorithm = input[0];
    static Grid grid = CreateGrid(input.Skip(2));
    public object Part1() => Cycle(algorithm).Skip(1).First().Pixels.Values.Count(c => c == '#');
    public object Part2() => Cycle(algorithm).Skip(49).First().Pixels.Values.Count(c => c == '#');

    IEnumerable<Grid> Cycle(string algorithm)
    {
        bool odd = true;
        while (true)
        {
            grid = grid.Enhance(algorithm, odd);
            yield return grid;
            odd = !odd;
        }
    }

    internal static Grid CreateGrid(IEnumerable<string> input)
    {
        var q = from line in input.Select((s, y) => (s, y))
                from c in line.s.Select((c, x) => (c, x))
                select (c.x, line.y, c.c);
        return new Grid(q.ToImmutableDictionary(c => new Coordinate(c.x,c.y), c => c.c));

    }
}

record Coordinate(int x, int y)
{
    public override string ToString() => $"({x}, {y})";
    public IEnumerable<Coordinate> Neighbors() => from dy in Range(-1, 3)
                                                  from dx in Range(-1, 3)
                                                  select new Coordinate(x + dx, y + dy);
}


record Grid(ImmutableDictionary<Coordinate, char> Pixels)
{
    public Grid Enhance(string algorithm, bool odd)
    {
        var minx = Pixels.Keys.Min(p => p.x);
        var miny = Pixels.Keys.Min(p => p.y);
        var maxx = Pixels.Keys.Max(p => p.x);
        var maxy = Pixels.Keys.Max(p => p.y);

        var pixels = (from y in Range(miny - 1, maxy - miny + 3)
                      from x in Range(minx - 1, maxx - minx + 3)
                      let c = new Coordinate(x, y)
                      let index = GetAddress(c, odd)
                      let p = algorithm[index]
                      select (c,p)).ToImmutableDictionary(x => x.c, x => x.p);
        return new Grid(pixels);
    }

    private int GetAddress(Coordinate c, bool odd)
    {
        var defaultValue = odd ? 0 : 1;
        var key = new string((from n in c.Neighbors() select this[n, odd ? '.' : '#'] == '#' ? '1' : '0').ToArray());
        return Convert.ToInt32(key, 2);

    }

    private char this[Coordinate c, char defaultValue = '.'] => Pixels.ContainsKey(c) ? Pixels[c] : defaultValue;
  
}
