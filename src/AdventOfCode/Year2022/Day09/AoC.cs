using AdventOfCode.Year2022.Day05;

namespace AdventOfCode.Year2022.Day09;
public class AoC202209
{
    static string[] input = Read.SampleLines();
    private IEnumerable<char> movements = from line in input
                                                            let d = line[0]
                                                            let l = int.Parse(line[2..])
                                                            from n in Repeat(1, l)
                                                            select d;
    public int Part1() 
    {
        Point h = new(), t = new();

        HashSet<Point> visited = new();
        visited.Add(t);
        foreach (var d in movements)
        {
            h = h.Move(d);
            t = t.MoveTo(h, d);
            visited.Add(t);
        }
        return visited.Count;
    }


    public int Part2()
    {
        Point h = new(), t1 = new(), t2 = new(), t3 = new(), t4 = new(), t5 = new(), t6 = new(), t7 = new(), t8 = new(), t9 = new();

        HashSet<Point> visited = new();
        visited.Add(t9);
        foreach (var d in movements)
        {
            Console.WriteLine(d);
            h = h.Move(d);
            t1 = t1.MoveTo(h, d);
            t2 = t2.MoveTo(t1, d);
            t3 = t3.MoveTo(t2, d);
            t4 = t4.MoveTo(t3, d);
            t5 = t5.MoveTo(t4, d);
            t6 = t6.MoveTo(t5, d);
            t7 = t7.MoveTo(t6, d);
            t8 = t8.MoveTo(t7, d);
            t9 = t9.MoveTo(t8, d);
            visited.Add(t9);
            Console.WriteLine(t9);
        }
        return visited.Count;
    }
}

readonly record struct Point(int x, int y)
{
    public Point Move(char d) => d switch
    {
        'R' => this with { x = x + 1 },
        'L' => this with { x = x - 1 },
        'U' => this with { y = y + 1 },
        'D' => this with { y = y - 1 },
        _ => throw new Exception("unexpected input")
    };
    public Point MoveTo(Point h, char d) => Toofar(h) ? d switch
    {
        'R' => this with { x = h.x > x ? h.x - 1 : h.x + 1, y = h.y },
        'L' => this with { x = h.x < x ? h.x + 1 : h.x - 1, y = h.y },
        'U' => this with { x = h.x, y = h.y > y ? h.y - 1 : h.y},
        'D' => this with { x = h.x, y = h.y < y ? h.y + 1 : h.y},
        _ => throw new Exception("unexpected input")
    } : this;

    private bool Toofar(Point other)
        => Math.Abs(other.x - x) > 1 || Math.Abs(other.y - y) > 1;

    public override string ToString() => $"({x},{y})";
}

public class Tests
{
    [Theory]
    [InlineData(0, 0, 0, 0, 'R', 4, 4, 0, 3, 0)]
    [InlineData(4, 0, 3, 0, 'U', 4, 4, 4, 4, 3)]
    [InlineData(4, 4, 4, 3, 'L', 3, 1, 4, 2, 4)]
    [InlineData(1, 4, 2, 4, 'D', 1, 1, 3, 2, 4)]
    [InlineData(1, 3, 2, 4, 'R', 4, 5, 3, 4, 3)]
    [InlineData(5, 3, 4, 3, 'D', 1, 5, 2, 4, 3)]
    [InlineData(5, 2, 4, 3, 'L', 5, 0, 2, 1, 2)]
    [InlineData(0, 2, 1, 2, 'R', 2, 2, 2, 1, 2)]
    public void Test(int hx1, int hy1, int tx1, int ty1, char d, int l, int hx2, int hy2, int tx2, int ty2)
    {
        var h = new Point(hx1, hy1);
        var t = new Point(tx1, ty1);

        var expected = (h: new Point(hx2, hy2), t: new Point(tx2, ty2));

        foreach (var i in Enumerable.Repeat(1, l))
        {
            h = h.Move(d);
            t = t.MoveTo(h, d);
        }

        Assert.Equal(expected, (h, t));
    }
}