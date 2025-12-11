using System.Runtime.CompilerServices;

namespace AdventOfCode.Year2025.Day09;

record struct Coordinate(int x, int y)
{
    public static Coordinate Parse(ReadOnlySpan<char> s)
    {
        Range[] ranges = new Range[2];
        s.Split(ranges, ',');
        return new Coordinate(int.Parse(s[ranges[0]]), int.Parse(s[ranges[1]]));
    }

    public static Coordinate operator+(Coordinate c, (int dx, int dy) p) => new(c.x + p.dx, c.y + p.dy);

    public override string ToString()
    {
        return $"({x},{y})";
    }
}

record struct Rectangle(Coordinate TopLeft, Coordinate BottomRight)
{
    public long Height => BottomRight.y - TopLeft.y + 1;
    public long Width => BottomRight.x - TopLeft.x + 1;

    public long Area => Width * Height;
    public static Rectangle FromCoordinates(Coordinate a, Coordinate b)
    {
        var topLeft = new Coordinate(Min(a.x, b.x), Min(a.y, b.y));
        var bottomRight = new Coordinate(Max(a.x, b.x), Max(a.y, b.y));
        return new Rectangle(topLeft, bottomRight);
    }

    public Coordinate BottomLeft => TopLeft with { y = BottomRight.y };
    public Coordinate TopRight => BottomRight with { y = TopLeft.y };

    // check if this rectangle is crossed by the given edge
    public bool Intersects(Edge edge) => Edges.Any(edge.Crosses);

    public Edge[] Edges
    {
        get {
            return field ??= [
                new Edge(TopLeft, TopRight),
                new Edge(TopRight, BottomRight),
                new Edge(BottomRight, BottomLeft),
                new Edge(BottomLeft, TopLeft)
            ];
        }
    }


}

record struct Edge(Coordinate Start, Coordinate End)
{
    public readonly bool IsVertical => Start.x == End.x;
    public readonly bool IsHorizontal => Start.y == End.y;

    // Manhattan distance between Start & End
    public readonly int Length => Abs(Start.x - End.x) + Abs(Start.y - End.y);
    public IEnumerable<Coordinate> Points
    {
        get {
            if (IsVertical)
            {
                for (int y = Min(Start.y, End.y); y <= Max(Start.y, End.y); y++)
                {
                    yield return new Coordinate(Start.x, y);
                }
            }
            else if (IsHorizontal)
            {
                for (int x = Min(Start.x, End.x); x <= Max(Start.x, End.x); x++)
                {
                    yield return new Coordinate(x, Start.y);
                }
            }
        }
    }
    public readonly bool Contains(Coordinate c) =>
        (IsVertical && c.x == Start.x && Min(Start.y, End.y) <= c.y && c.y <= Max(Start.y, End.y)) ||
        (IsHorizontal && c.y == Start.y && Min(Start.x, End.x) <= c.x && c.x <= Max(Start.x, End.x));

    public readonly bool Crosses(Edge other) => 
        (IsVertical && other.IsHorizontal &&
            Min(other.Start.x, other.End.x) < Start.x && Start.x < Max(other.Start.x, other.End.x) &&
            Min(Start.y, End.y) < other.Start.y && other.Start.y < Max(Start.y, End.y))
        ||
        (IsHorizontal && other.IsVertical &&
            Min(other.Start.y, other.End.y) < Start.y && Start.y < Max(other.Start.y, other.End.y) &&
            Min(Start.x, End.x) < other.Start.x && other.Start.x < Max(Start.x, End.x));

    public override string ToString()
    {
        return $"{Start} -> {End}";
    }
}
record struct Polygon(Coordinate[] Corners)
{
    Rectangle? _field;
    public Rectangle BoundingBox     {
        get {
            return _field ??= new (new (Corners.Min(c => c.x), Corners.Min(c => c.y)), new (Corners.Max(c => c.x), Corners.Max(c => c.y)));
        }
    }

    public HashSet<Edge> Edges
    {
        get {
            return field ??= [.. GetEdges().OrderByDescending(e => e.Length)];
        }
    }

    private IEnumerable<Edge> GetEdges()
    {
        for (int i = 0; i < Corners.Length; i++)
        {
            var start = Corners[i];
            var end = Corners[(i + 1) % Corners.Length];
            yield return new Edge(start, end);
        }
    }

    // a rectangle is valid if
    // * all corners are 'inside'
    // * all corners of a rectangle reduced in size by 1 are 'inside'
    // * no edges of the polygon cross into the rectangle
    public bool IsValid(Rectangle rectangle)
    {
        var p = this;

        if (!p.IsInside(rectangle.TopLeft))
            return false;
        if (!p.IsInside(rectangle.BottomRight))
            return false;
        if (!p.IsInside(rectangle.TopRight))
            return false;
        if (!p.IsInside(rectangle.BottomLeft))
            return false;

        if (rectangle.Height > 1 && rectangle.Width > 1)
        {
            if (!p.IsInside(rectangle.TopLeft + (1, 1)))
                return false;
            if (!p.IsInside(rectangle.TopRight + (-1, 1)))
                return false;
            if (!p.IsInside(rectangle.BottomRight + (-1, -1)))
                return false;
            if (!p.IsInside(rectangle.BottomLeft + (1, -1)))
                return false;
        }
        foreach (var edge in p.Edges)
        {
            if (rectangle.Intersects(edge))
                return false;
        }
        return true;
    }

    private bool IsInside(Coordinate p)
    {
        // check if on boundary
        foreach (var edge in Edges)
        {
            if (edge.Contains(p))
                return true;
        }

        // ray-casting to the right of p; 
        // count how many edges it intersects
        // if odd, point is inside; if even, point is outside
        int count = 0;
        // if it is a vertical edge & the point is to the left of it
        foreach (var e in Edges.Where(e => e.IsVertical && p.x < e.Start.x))
        {
            var point = new Coordinate(e.Start.x, p.y); // point on the line of the edge
            if (e.Contains(point))
            {
                count++;
            }
        }

        if (count % 2 != 1) return false;
        return true;

    }
}

public class AoC202509(string[] input)
{
    public AoC202509() : this(Read.InputLines()) { }

    Coordinate[] coordinates = [.. from line in input
                                   select Coordinate.Parse(line.AsSpan()) ];

    public long Part1()
    {
        var rectangles = (
            from i in Range(0, coordinates.Length)
            from j in Range(i + 1, coordinates.Length - i - 1)
            let rectangle = Rectangle.FromCoordinates(coordinates[i], coordinates[j])
            orderby rectangle.Area descending
            select rectangle
            );

        return rectangles.First().Area;
    }
    public long Part2()
    {
        var polygon = new Polygon(coordinates);
        var rectangles = (
            from i in Range(0, coordinates.Length)
            from j in Range(i + 1, coordinates.Length - i - 1)
            let rectangle = Rectangle.FromCoordinates(coordinates[i], coordinates[j])
            orderby rectangle.Area descending
            select rectangle
            ).ToArray();

        return (
            from rectangle in rectangles
            where polygon.IsValid(rectangle)
            select rectangle
            ).First().Area;

    }

}

public class AoC202509Tests
{
    private readonly AoC202509 sut;
    public AoC202509Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202509(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(50, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(24, sut.Part2());
    }

    [Fact]
    public void RectangleArea_CalculatesCorrectly()
    {
        var rect = Rectangle.FromCoordinates(new Coordinate(7, 1), new Coordinate(11, 7));
        Assert.Equal(35, rect.Area);
    }

    [Fact]
    public void Rectangle_Edges_AreCalculatedCorrectly()
    {
        var topleft = new Coordinate(1, 1);
        var topright = new Coordinate(3, 1);
        var bottomright = new Coordinate(3, 3);
        var bottomleft = new Coordinate(1, 3);

        var edges = new Edge[]
        {
            new(topleft, topright),
            new(topright, bottomright),
            new(bottomright, bottomleft),
            new(bottomleft, topleft)
        };

        var rect = Rectangle.FromCoordinates(topleft, bottomright);

        Assert.Equal(edges, rect.Edges.ToArray());
    }

    [Fact]
    public void Polygon_Contains_WorksCorrectly()
    {
        // (11,0),(30,0),(30,2),(27,2),(27,7),(23,7),(23,2),(15,2),(15,4),(19,4),(19,9),(0,9),(0,4),(11,4)
        var coordinates = new Coordinate[]
        {
            new Coordinate(11,0),
            new Coordinate(30,0),
            new Coordinate(30,2),
            new Coordinate(27,2),
            new Coordinate(27,7),
            new Coordinate(23,7),
            new Coordinate(23,2),
            new Coordinate(15,2),
            new Coordinate(15,4),
            new Coordinate(19,4),
            new Coordinate(19,9),
            new Coordinate(0,9),
            new Coordinate(0,4),
            new Coordinate(11,4)
        };
        var polygon = new Polygon(coordinates);

    }


    /*
     *
     *
     *       0         11  15  19  23  27 30 
     *       0123456789012345678901234567890
     *  0  0            +-------s---s------+
     *     1            |                  |      
     *  2  2            s   +---s---+ r.+--+.   -> crosses 1 edge (odd) => inside
     *     3            |   |       |   |
     *  4  4 +----------+   +---+   s   +--+
     *     5 |                  | q.|......|.   -> crosses 2 edges => outside
     *     6 |        p.........|...|......|.   -> crosses 3 edges => inside
     *  7  7 s                  s   +------+
     *     8 |                  |
     *  9  9 +----------s-------+ 
     *     
     *     (11,0),(30,0),(30,2),(27,2),(27,4),(30,4),(30,7),(23,7),(23,2),(15,2),(15,4),(19,4),(19,9),(0,9),(0,4),(11,4)
     *     
     *     the rectangle (15,2) to (30,4) is invalid because some segments of it's edges are outside the polygon
     *     
     *       0         11  15  19  23  27 30 
     *       0123456789012345678901234567890
     *  0  0            +------------------+
     *     1            |                  |      
     *  2  2            |   +-------+      |
     *     3            |   |       |      |
     *  4  4 +----------+   +---+   |      |
     *     5 |                  | q.|......|.  
     *     6 |        p.........|...|......|.  
     *  7  7 |                  |   +------+
     *     8 |                  |
     *  9  9 +------------------+ 
     *
     */
    [Fact]
    public void Test()
    {
        // (11,0),(30,0),(30,2),(27,2),(27,4),(30,4),(30,7),(23,7),(23,2),(15,2),(15,4),(19,4),(19,9),(0,9),(0,4),(11,4)
        // the rectangle (15,2) to (30,4) is invalid because some segments of it's edges are outside the polygon

        var coordinates = new Coordinate[] {
            new Coordinate(11,0),
            new Coordinate(30,0),
            new Coordinate(30,2),
            new Coordinate(27,2),
            new Coordinate(27,7),
            new Coordinate(23,7),
            new Coordinate(23,2),
            new Coordinate(15,2),
            new Coordinate(15,4),
            new Coordinate(19,4),
            new Coordinate(19,9),
            new Coordinate(0,9),
            new Coordinate(0,4),
            new Coordinate(11,4)
            };

        var polygon = new Polygon(coordinates);
        var rectangle = Rectangle.FromCoordinates(new Coordinate(15, 2), new Coordinate(30, 4));
        Assert.False(polygon.IsValid(rectangle));
    }

    [Fact]
    public void AnalyzeInput_AllCoordinatesAreCornersOfPolygon()
    {
        var input = File.ReadAllLines("D:\\dev\\jhgbrt\\advent-of-code\\src\\AdventOfCode\\Year2025\\Day09\\input.txt").Select(l => Coordinate.Parse(l.AsSpan())).ToArray();
        for (int i = 0; i < input.Length - 1; i++)
        {
            var a = input[i];
            var b = input[i + 1];
            if (a.x != b.x && a.y != b.y)
            {
                throw new Exception($"Coordinate {i} to {i + 1} does not switch direction: {a} to {b}");
            }
        }
    }

    [Fact]
    public void Edge_Contains_WorksCorrectly()
    {
        var edge = new Edge(new Coordinate(1, 1), new Coordinate(1, 5));

        for (int y = 1; y <= 5; y++)
        {
            Assert.True(edge.Contains(new Coordinate(1, y)));
        }
        Assert.False(edge.Contains(new Coordinate(2, 3)));

        edge = new Edge(new Coordinate(2, 2), new Coordinate(5, 2));
        for (int x = 2; x <= 5; x++)
        {
            Assert.True(edge.Contains(new Coordinate(x, 2)));
        }
        Assert.False(edge.Contains(new Coordinate(3, 3)));
    }

    [Fact]
    public void Rectangle_Intersects_WorksCorrectly()
    {
        var rectangle = Rectangle.FromCoordinates(new Coordinate(1, 1), new Coordinate(4, 4));

        // horizontal edges
        Assert.True(rectangle.Intersects(new Edge(new Coordinate(0, 2), new Coordinate(5, 2))));
        Assert.True(rectangle.Intersects(new Edge(new Coordinate(0, 2), new Coordinate(3, 2))));
        Assert.True(rectangle.Intersects(new Edge(new Coordinate(3, 2), new Coordinate(5, 2))));
        Assert.False(rectangle.Intersects(new Edge(new Coordinate(0, 1), new Coordinate(1, 1))));
        Assert.False(rectangle.Intersects(new Edge(new Coordinate(0, 0), new Coordinate(0, 5))));

        // vertical edges
        Assert.True(rectangle.Intersects(new Edge(new Coordinate(2, 0), new Coordinate(2, 5))));
        Assert.True(rectangle.Intersects(new Edge(new Coordinate(2, 0), new Coordinate(2, 3))));
        Assert.True(rectangle.Intersects(new Edge(new Coordinate(2, 3), new Coordinate(2, 5))));
        Assert.False(rectangle.Intersects(new Edge(new Coordinate(0, 0), new Coordinate(5, 0))));


    }
}

