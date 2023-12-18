using Spectre.Console;

using System.Security.Cryptography;

namespace AdventOfCode.Year2023.Day18;
public class AoC202318
{
    public AoC202318():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableArray<Item> items;
    internal IEnumerable<Item> Items => items;
    public AoC202318(string[] input, TextWriter writer)
    {
        items = input.Select(s => Regexes.MyRegex().As<Item>(s)).ToImmutableArray();
        this.writer = writer;
    }

    public object Part1()
    {
        //var g = new InfiniteGrid();
        var list = new List<Coordinate>();
        var c = Coordinate.Origin;
        list.Add(c);
        foreach (var item in items)
        {
           for (var i = 0; i < item.n; i++)
           {
               c = item.dir switch
               {
                   'R' => c.E,
                   'L' => c.W,
                   'U' => c.N,
                   'D' => c.S
               };
               list.Add(c);
           }
        }

        //var rows = (
        //    from item in list
        //    group item by item.y into g
        //    let row = g.Key
        //    let points = g.Select(_ => _.x).Order().ToList()
        //    select points).ToList();

        //foreach (var row in rows)
        //{
        //    var i = 0;
        //    while (i < row.Count - 2)
        //    {
        //        while (row[i + 1] == row[i] + 1) i++;
        //    }
        //}

        return -1;
    }

    internal IEnumerable<Range> Ranges(IList<int> row)
    {
        if (row[0] > 0)
            yield return new(0, row[0]);
        var start = row[0];
        var previous = -1;
        var enumerator = row.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            if (current == previous + 1)
            {
                previous = current;
            }
            else if (previous > 0)
            {
                yield return new Range(start, previous + 1);
                yield return new Range(previous + 1, current);
                start = current;
                previous = current;
            }
            else
            {
            }
        }
        yield return new(start, row[^1] + 1);
    }

    public object Part2() => "";

    private bool IsInsideLoop(FiniteGrid grid, Coordinate p) => CountLinesCrossed(grid, p) % 2 == 0;

    int CountLinesCrossed(FiniteGrid grid, Coordinate p)
    {
        Func<State, Coordinate, Result> f = NotOnLine;
        var state = new State(grid, p, 0);
        var e = East(grid, p).GetEnumerator();
        while (e.MoveNext())
        {
            (f, state) = f(state, e.Current);
        }
        return state.Count;
    }

    static IEnumerable<Coordinate> East(FiniteGrid grid, Coordinate c)
    {
        var current = c;
        while (current.x < grid.Width)
        {
            yield return current;
            current = current.E;
        }
    }

    Result NotOnLine(State state, Coordinate c) => state.Grid[c] switch
    {
        '#' => new(OnLine, state),
        '.' => new(NotOnLine, state)
    };

    Result OnLine(State state, Coordinate c) => state.Grid[c] switch
    {
        '#' => new(OnLine, state),
        '.' => new(NotOnLine, state.AddCount()),
    };


    record struct Result(Func<State, Coordinate, Result> Next, State State);

    record struct State(FiniteGrid Grid, Coordinate p, int Count)
    {
        public State AddCount() => this with { Count = Count + 1 };
    }

}

readonly record struct Item(char dir, int n, string color);

static partial class Regexes
{
    [GeneratedRegex(@"^(?<dir>[RLUD]) (?<n>\d+) \((?<color>).*\)$")]
    public static partial Regex MyRegex();
}

public class AoC202318Tests
{
    private readonly AoC202318 sut;
    public AoC202318Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202318(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(14, sut.Items.Count());
        Assert.Equal(6, sut.Items.First().n);
        Assert.Equal('R', sut.Items.First().dir);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(62, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(string.Empty, sut.Part2());
    }

    [Fact]
    public void Ranges1()
    {
        var input = new[] { 0, 1, 2, 5, 6 };
        var ranges = sut.Ranges(input);
        Assert.Equal(new Range[] { new(0, 3), new(3, 5), new(5, 7) }, ranges);
    }

    [Fact]
    public void Ranges2()
    {
        var input = new[] { 2, 3, 6 };
        var ranges = sut.Ranges(input);
        Assert.Equal(new Range[] { new(0, 2), new(2, 4), new(4, 7) }, ranges);
    }

}

record struct Range(int start, int end)
{
}