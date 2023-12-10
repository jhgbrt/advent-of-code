using AdventOfCode.Common;

namespace AdventOfCode.Year2023.Day10;
public class AoC202310
{
    static string[] input = Read.InputLines();
    static (Coordinate start, FiniteGrid grid) x = CreateGrid(input);
    public object Part1() => Part1(input);

    public int Part1(string[] input)
    {
        var (start, g) = x;
        var path = FindPath(g, start).ToList();
        return path.Count % 2 == 0 ? path.Count / 2 : path.Count / 2 - 1;

    }

    static (Coordinate start, FiniteGrid grid) CreateGrid(string[] input)
    {
        FiniteGrid grid = new FiniteGrid(input);
        Coordinate start = grid.Find('S');
        return (start, SetStartConnection(grid, start));
    }

    private IEnumerable<Coordinate> FindPath(FiniteGrid grid, Coordinate start)
    {        
        var current = start;
        var next = ConnectedNeighbours(grid, current).First();
        Coordinate? previous = null;
        do
        {
            yield return current;
            next = ConnectedNeighbours(grid, current).First(c => c != previous);
            previous = current;
            current = next;
        } while (current != start);
    }


    static FiniteGrid SetStartConnection(FiniteGrid grid, Coordinate start)
    {
        var pipe = (from x in grid.Neighbours(start)
                    where (x.d, grid[x.c]) switch
                    {
                        (Direction.N, '|' or 'F' or '7') => true,
                        (Direction.E, '-' or 'J' or '7') => true,
                        (Direction.S, '|' or 'J' or 'L') => true,
                        (Direction.W, '-' or 'F' or 'L') => true,
                        _ => false
                    }
                    orderby x.d
                    select x.d
       ).ToTuple2() switch
        {
            (Direction.N, Direction.E) => 'L',
            (Direction.N, Direction.S) => '|',
            (Direction.N, Direction.W) => 'J',
            (Direction.E, Direction.S) => 'F',
            (Direction.E, Direction.W) => '-',
            (Direction.S, Direction.W) => '7',
        };

        return grid.With(b => b[start] = pipe);
    }

    public object Part2() => Part2(input);

    public object Part2(string[] input)
    {
        var (start, g) = CreateGrid(input);

        var path = FindPath(g, start).ToHashSet();

        var query = from p in g.Points()
                    where !path.Contains(p)
                    let count = CountLinesCrossed(g, p, path)
                    where count % 2 == 1
                    select p;
        return query.Count();

    }

    int CountLinesCrossed(FiniteGrid g, Coordinate p, HashSet<Coordinate> loop)
    {
        Func<State, Coordinate, Result> f = NotOnLoop;
        var state = new State(g, p, loop);

        var east = g.East(p);
        var e = east.GetEnumerator();
        while (e.MoveNext())
        {
            (f, state) = f(state, e.Current);
        }
        return state.Count;
    }

    Result NotOnLoop(State state, Coordinate c)
    {
        if (state.Loop.Contains(c))
            return new(OnLoop, state.AddCount());
        else
            return new(NotOnLoop, state);
    }
    Result OnLoop(State state, Coordinate c)
    {
        return state.Grid[c] switch
        {
            '-' => new(OnLoop, state),
            _ => new (NotOnLoop, state)
        };
    }

    record struct Result(Func<State,Coordinate,Result> Next, State State);
    
    class State(FiniteGrid grid, Coordinate p, HashSet<Coordinate> loop)
    {
        public int Count { get; private set; }
        public State AddCount() { Count++; return this; }
        public FiniteGrid Grid => grid;
        public HashSet<Coordinate> Loop => loop;

    }

    IEnumerable<int> CountBlocksWest(FiniteGrid g, Coordinate p, HashSet<Coordinate> loop)
    {
        var east = g.West(p);
        var e = east.GetEnumerator();
        int groupNr = 1;
        bool inloop = true;
        while (e.MoveNext())
        {
            if (loop.Contains(e.Current) != inloop)
            {
                inloop = !inloop;
                groupNr++;
            }
            if (inloop)
                yield return groupNr;
        }

    }

    private IEnumerable<Coordinate> ConnectedNeighbours(FiniteGrid grid, Coordinate coordinate)
    => grid[coordinate] switch
    {
        '|' => [coordinate.N, coordinate.S],
        '-' => [coordinate.E, coordinate.W],
        'L' => [coordinate.N, coordinate.E],
        'J' => [coordinate.N, coordinate.W],
        'F' => [coordinate.S, coordinate.E],
        '7' => [coordinate.S, coordinate.W],
        _ => Array.Empty<Coordinate>()
    };


}


public class Tests
{
    [Theory]
    [InlineData(2, 4)]
    [InlineData(3, 8)]
    [InlineData(4, 10)]
    public void Part2(int sample, int expected)
    {
        var input = Read.Sample(sample).Lines().ToArray();
        var aoc = new AoC202310();
        Assert.Equal(expected, aoc.Part2(input));
    }
}
