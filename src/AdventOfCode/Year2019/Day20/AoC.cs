using Net.Code.Graph;
using Net.Code.Graph.Algorithms;

using System.Collections;
using System.Diagnostics.CodeAnalysis;
using System.Reflection.Emit;

namespace AdventOfCode.Year2019.Day20;

public class AoC201920(string[] input, TextWriter writer)
{
    public AoC201920() : this(Read.InputLines(), Console.Out)
    {
    }

    public object Part1()
    {
        var grid = new Grid(input);
        var donut = Donut.From(grid);

        var letters = (
            from pos in grid.Keys
            let c = grid[pos]
            where char.IsLetter(c)
            select (pos, value: c)
        );

        var labels = (
            from a in letters
            from b in letters
            where a.pos.S == b.pos || a.pos.E == b.pos
            select new Label(a.pos, b.pos, $"{a.value}{b.value}")
            ).ToList();

        var ports = 
            from p in labels
            group p by p.label into g
            where g.Skip(1).Any()
            let first = g.First().GetPosition(donut)
            let second = g.Skip(1).First().GetPosition(donut)
            select (label: g.Key, first, second);

        var labelsByPosition = (
            from p in labels
            let position = p.GetPosition(donut)
            select (p, position)
            ).ToDictionary(x => x.position, x => x.p.label);

        var graph = GraphBuilder.Create<Coordinate, int>()
            .AddVertices(
                grid.Keys.Where(c => grid[c] == '.').ToList()
            )
            .AddEdges(
                from p in grid.Keys.Where(c => grid[c] == '.')
                from n in grid.Neighbours(p).Where(c => grid[c] == '.')
                select Edge.Create(p, n, 1)
            )
            .AddEdges(
                from p in ports
                select Edge.Create((p.first, p.second), 1)
            )
            .AddEdges(
                from p in ports
                select Edge.Create((p.second, p.first), 1)
            )
            .WithLabels(labelsByPosition)
            .BuildGraph();

        var start = labels.First(p => p.label == "AA").GetPosition(donut);
        var end = labels.First(p => p.label == "ZZ").GetPosition(donut);
        var result = Dijkstra.ShortestPaths(graph, start);
        var path = result.GetPath(end);

        GC.KeepAlive(writer);
        return path.Count();
    }
    public object Part2()
    {
        var grid = new Grid(input);
        var donut = Donut.From(grid);

        var letters = (
            from pos in grid.Keys
            let c = grid[pos]
            where char.IsLetter(c)
            select (pos, value: c)
        );

        var labels = (
            from a in letters
            from b in letters
            where a.pos.S == b.pos || a.pos.E == b.pos
            select new Label(a.pos, b.pos, $"{a.value}{b.value}")
        ).ToList();

        var ports =
            (from p in labels
             group p by p.label into g
             where g.Skip(1).Any()
             let first = g.First().GetPosition(donut)
             let second = g.Skip(1).First().GetPosition(donut)
             select (label: g.Key, first, second)).ToList();

        var start = labels.First(p => p.label == "AA").GetPosition(donut);
        var end = labels.First(p => p.label == "ZZ").GetPosition(donut);

        var portalByPosition = new Dictionary<Coordinate, (Coordinate target, bool isOuter)>();
        foreach (var p in ports)
        {
            if (p.label is "AA" or "ZZ") continue; // AA/ZZ are start/end only
            // Outer portals are adjacent to the outer border of the map; inner portals are deeper inside.
            bool IsOuter(Coordinate c)
                => c.x <= 2 || c.y <= 2 || c.x >= grid.Width - 3 || c.y >= grid.Height - 3;

            var firstIsOuter = IsOuter(p.first);
            var secondIsOuter = IsOuter(p.second);
            portalByPosition[p.first] = (p.second, isOuter: firstIsOuter);
            portalByPosition[p.second] = (p.first, isOuter: secondIsOuter);
        }

        var q = new Queue<(Coordinate pos, int level, int dist)>();
        var seen = new HashSet<(Coordinate pos, int level)>();
        q.Enqueue((start, 0, 0));
        seen.Add((start, 0));

        while (q.Count > 0)
        {
            var (pos, level, dist) = q.Dequeue();
            if (pos == end && level == 0)
            {
                return dist;
            }

            // Move to neighbours
            foreach (var n in grid.Neighbours(pos))
            {
                if (grid[n] != '.') continue;
                var state = (n, level);
                if (seen.Add(state)) q.Enqueue((n, level, dist + 1));
            }

            // Use portal if available
            if (portalByPosition.TryGetValue(pos, out var portal))
            {
                var nextLevel = portal.isOuter ? level - 1 : level + 1;
                if (nextLevel >= 0)
                {
                    var state = (portal.target, nextLevel);
                    if (seen.Add(state)) q.Enqueue((portal.target, nextLevel, dist + 1));
                }
            }
        }

        writer.WriteLine("no solution");

        return -1; // no path
    }



}

class Grid : IReadOnlyDictionary<Coordinate, char>
{

    //        x
    //   +---->
    //   |
    //   |
    // y v

    readonly ImmutableDictionary<Coordinate, char> items;
    readonly Coordinate origin = new(0, 0);
    readonly Coordinate endmarker;
    readonly char empty;
    public int Height => endmarker.y;
    public int Width => endmarker.x;

    public IEnumerable<Coordinate> Keys
    {
        get
        {
            for (int y = origin.y; y < Height; y++)
                for (int x = origin.x; x < endmarker.x; x++)
                    yield return new Coordinate(x, y);
        }
    }

    public IEnumerable<char> Values => Keys.Select(k => this[k]);

    public int Count => Width * Height;

    public Grid(string[] input, char empty = '.')
    : this(ToDictionary(input, empty), empty, new(input[0].Length, input.Length))
    {
    }
    static ImmutableDictionary<Coordinate, char> ToDictionary(string[] input, char empty)
    => (from y in Range(0, input.Length)
        from x in Range(0, input[y].Length)
        where input[y][x] != empty
        select (x, y, c: input[y][x])).ToImmutableDictionary(t => new Coordinate(t.x, t.y), t => t.c);

    internal Grid(ImmutableDictionary<Coordinate, char> items, char empty, Coordinate endmarker)
    {
        this.items = items;
        this.empty = empty;
        this.endmarker = endmarker;
    }
    public char this[Coordinate p] => items.TryGetValue(p, out var c) ? c : empty;

    public IEnumerable<Coordinate> Points() =>
        from y in Range(origin.y, endmarker.y)
        from x in Range(origin.x, endmarker.x)
        select new Coordinate(x, y);



    readonly static (int x, int y)[] deltas = [(0, -1), (1, 0), (0, 1), (-1, 0)];
    public IEnumerable<Coordinate> Neighbours(Coordinate p)
    {
        foreach (var (dx, dy) in deltas)
        {
            var c = new Coordinate(p.x + dx, p.y + dy);
            if (ContainsKey(c)) yield return c;
        }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = origin.y; y < endmarker.y; y++)
        {
            for (int x = origin.x; x < endmarker.x; x++) sb.Append(this[new(x, y)]);
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public bool Contains(Coordinate c) => ContainsKey(c);

    public bool ContainsKey(Coordinate key) => key.x >= 0 && key.y >= 0 && key.x < endmarker.x && key.y < endmarker.y;

    public bool TryGetValue(Coordinate key, [MaybeNullWhen(false)] out char value)
    {
        if (ContainsKey (key))
        {
            value = this[key];
            return true;
        }
        value = default;
        return false;
    }

    public IEnumerator<KeyValuePair<Coordinate, char>> GetEnumerator() => Keys.Select(k => new KeyValuePair<Coordinate, char>(k, this[k])).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
}

readonly record struct Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";
    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
    public static Coordinate operator -(Coordinate left, (int dx, int dy) p) => new(left.x - p.dx, left.y - p.dy);
    public Coordinate S => this with { y = y + 1 };
    public Coordinate E => this with { x = x + 1 };
    public Coordinate W => this with { x = x - 1 };
    public Coordinate N => this with { y = y - 1 };
}


public class AoC202019Tests(ITestOutputHelper output)
{
    [Fact]
    public void TestPart1()
    {
        var aoc = new AoC201920(Read.SampleLines(1), new TestWriter(output));
        Assert.Equal(23, aoc.Part1());
    }
    [Fact]
    public void TestPart2()
    {
        var aoc = new AoC201920(Read.SampleLines(1), new TestWriter(output));
        Assert.Equal(26, aoc.Part2());
    }

    [Theory]
    [InlineData("AB", 13, 2)]
    [InlineData("CD", 2, 4)]
    [InlineData("EF", 5, 3)]
    [InlineData("IJ", 16, 5)]
    [InlineData("KL", 17, 6)]
    [InlineData("GH", 3, 7)]
    [InlineData("MN", 5, 14)]
    [InlineData("OP", 13, 15)]
    public void TestLetters(string label, int x, int y)
    {

        // labels
        //     00000000001111111111
        //     01234567890123456789
        // 00               A
        // 01               B                  AB -> (13,2)
        // 02    ###########.####              CD -> (2,4)
        // 03    ###.############              EF -> (5,3)
        // 04  CD.# E          ##              IJ -> (16,5)
        // 05    ## F        IJ.#              KL -> (17,6)
        // 06    ##            #.KL            GH -> (3,7)
        // 07    #.GH          ##              MN -> (5,14)
        // 08    ##            ##              OP -> (13,15)
        // 09    ##            ##              
        // 10    ##            ##              
        // 11    ##            ##  
        // 12    ## M          ##  
        // 13    ## N          ##
        // 14    ###.############
        // 15    ###########.####
        // 16               O
        // 17               P
        // 18



        var input = """
                     A      
                     B      
          ###########.####  
          ###.############  
        CD.# E          ##  
          ## F        IJ.#  
          ##            #.KL
          #.GH          ##  
          ##            ##  
          ##            ##  
          ##            ##  
          ##            ##  
          ## M          ##  
          ## N          ##  
          ###.############  
          ###########.####  
                     O      
                     P      
        """;
        var lines = input.Split(Environment.NewLine);
        var letters = (
            from r in lines.Index()
            from c in r.Item.Index()
            where char.IsLetter(c.Item)
            && label.Contains(c.Item)
            select (pos: new Coordinate(c.Index, r.Index), value: c.Item)
        ).ToList();
        var donut = Donut.From(new Grid(lines));
        var ports = (
            from a in letters
            from b in letters
            where a.pos.S == b.pos || a.pos.E == b.pos
            select new Label(a.pos, b.pos, $"{a.value}{b.value}")
            ).ToDictionary(x => x.label, x => x.GetPosition(donut));

        Assert.Equal(new Coordinate(x, y), ports[label]);
    }

    public class DonutTests
    {
        [Fact]
        public void TestDonut()
        {
            var input = """
                         A      
                         B      
              ###########.####  
              ###.############  
            CD.# E          ##  
              ## F        IJ.#  
              ##            #.KL
              #.GH          ##  
              ##            ##  
              ##            ##  
              ##            ##  
              ##            ##  
              ## M          ##  
              ## N          ##  
              ###.############  
              ###########.####  
                         O      
                         P      
            """;
            var lines = input.Split(Environment.NewLine);
            var g = new Grid(lines);
            var donut = Donut.From(g);

            Assert.True(donut.IsOutside(new(0, 0)));
            Assert.True(donut.IsOutside(new(0, 1)));
            Assert.True(donut.IsOutside(new(1, 0)));
            Assert.True(donut.IsOutside(new(1, 1)));
            Assert.False(donut.IsOutside(new(2, 2)));
            Assert.False(donut.IsOutside(new(3, 3)));
            Assert.True(donut.IsInside(new(4, 4)));
            Assert.True(donut.IsInside(new(5, 4)));
            Assert.True(donut.IsInside(new(5, 5)));
            Assert.True(donut.IsInside(new(13, 13)));
            Assert.True(donut.IsInside(new(13, 12)));
            Assert.True(donut.IsInside(new(12, 13)));
            Assert.True(donut.Contains(new(3, 3)));
            Assert.True(donut.Contains(new(3, 14)));
            Assert.True(donut.Contains(new(3, 15)));
            Assert.False(donut.Contains(new(3, 16)));
            Assert.True(donut.Contains(new(14, 14)));
            Assert.True(donut.Contains(new(14, 15)));
            Assert.False(donut.Contains(new(14, 16)));

        }

    }



}

class Donut(Box outside, Box inside)
{
    public bool Contains(Coordinate c) => outside.Contains(c) && !inside.Contains(c);
    public bool IsOutside(Coordinate c)
        => !outside.Contains(c);
    public bool IsInside(Coordinate c)
        => inside.Contains(c);

    public static Donut From(Grid g)
    {

        /* width1 = outer width (= 2)
         * width2 = donut width (= 3)
         * 
         *     0123456789012345
         *    +----------------+
         *  0 |                |
         *  1 |                |
         *  2 |  ############  |
         *  3 |  ############  |
         *  4 |  ############  |
         *  5 |  ###      ###  |
         *  6 |  ###      ###  |
         *  7 |  ###      ###  |
         *  8 |  ############  |
         *  9 |  ############  |
         * 10 |  ############  |
         * 11 |                |
         * 12 |                |
         *    +----------------+
         * 
         * 
        */

        var c = new Coordinate(g.Width / 2, g.Height / 2);

        while (g[c] is not '#' and not '.') c = c.N;
        var y2 = c.y + 1;
        while (g[c] is '#' or '.') c = c.N;
        var y1 = c.y + 1;
        var width2 = y2 - y1;
        var width1 = y1;
        var topleft = new Coordinate(width1, width1);
        var bottomright = new Coordinate(g.Width - width1 - 1, g.Height - width1 - 1);
        var outside = new Box(topleft, bottomright);
        var inside = new Box(topleft + (width2, width2), bottomright - (width2, width2));
        return new Donut(outside, inside);
    }
}
record struct Box(Coordinate TopLeft, Coordinate BottomRight)
{
    public bool Contains(Coordinate c)
        => c.x >= TopLeft.x && c.x <= BottomRight.x && c.y >= TopLeft.y && c.y <= BottomRight.y;
}
record struct Label(Coordinate a, Coordinate b, string label)
{
    public Coordinate GetPosition(Donut donut)
    {
        Span<(int, int)> deltas = [(0, 1), (1, 0), (0, -1), (-1, 0)];
        foreach (var d in deltas)
        {
            if (donut.Contains(a + d)) return a + d;
            if (donut.Contains(b + d)) return b + d;
        }
        return new(-1, -1);
    }
}

