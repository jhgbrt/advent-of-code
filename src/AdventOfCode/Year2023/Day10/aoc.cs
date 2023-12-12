namespace AdventOfCode.Year2023.Day10;

public class AoC202310
{
    static string[] input = Read.InputLines();
    Coordinate start;
    FiniteGrid grid;
    HashSet<Coordinate> loop; 

    public AoC202310()
    {
        (start, grid, loop) = CreateGrid(input);
    }

    internal AoC202310(string[] input)
    {
        (start, grid, loop) = CreateGrid(input);
    }

    public object Part1() => loop.Count % 2 == 0 ? loop.Count / 2 : loop.Count / 2 - 1;

    static (Coordinate start, FiniteGrid grid, HashSet<Coordinate> loop) CreateGrid(string[] input)
    {
        FiniteGrid grid = new FiniteGrid(input);
        Coordinate start = grid.Find('S');
        var connection = DetermineConnection(grid, start);
        grid = grid.With(b => b[start] = connection);

        var loop = FindPath(grid, start).ToHashSet();
        grid = grid.With(
            b =>
            {
                b[start] = connection;
                var notinloop = b.Keys.Where(c => !loop.Contains(c)).ToList();
                foreach (var item in notinloop)
                {
                    b[item] = '.';
                }
            }
        );
        return (start, grid, loop);
    }

    static private IEnumerable<Coordinate> FindPath(FiniteGrid grid, Coordinate start)
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


    static char DetermineConnection(FiniteGrid grid, Coordinate start)
    {
        return (from x in grid.Neighbours(start)
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
    }
    public object Part2() => (from p in grid.Points()
                              where grid[p] == '.'
                              let count = CountLinesCrossed(grid, p, loop)
                              where count % 2 == 1
                              select p).Count();

    int CountLinesCrossed(FiniteGrid grid, Coordinate p, HashSet<Coordinate> loop)
    {
        Func<State, Coordinate, Result> f = OutsideLoop;
        var state = new State(grid, p, loop, 0);
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


    Result OutsideLoop(State state, Coordinate c) => state.Grid[c] switch
    {
        'F' => new(OutsideOnF, state),
        'L' => new(OutsideOnL, state),
        '|' => new(InsideLoop, state.AddCount()),
        '.' => new(OutsideLoop, state)
    };

    Result OutsideOnF(State state, Coordinate c) => state.Grid[c] switch
    {
        '-' => new(OutsideOnF, state),
        'J' => new(InsideLoop, state.AddCount()),
        '7' => new(OutsideLoop, state),
    };
    Result OutsideOnL(State state, Coordinate c) => state.Grid[c] switch
    {
        '-' => new(OutsideOnL, state),
        'J' => new(OutsideLoop, state),
        '7' => new(InsideLoop, state.AddCount())
    };
    Result InsideOnF(State state, Coordinate c) => state.Grid[c] switch
    {
        '-' => new(InsideOnF, state),
        'J' => new(OutsideLoop, state.AddCount()),
        '7' => new(InsideLoop, state),
    };
    Result InsideOnL(State state, Coordinate c) => state.Grid[c] switch
    {
        '-' => new(InsideOnL, state),
        'J' => new(InsideLoop, state),
        '7' => new(OutsideLoop, state.AddCount())
    };

    Result InsideLoop(State state, Coordinate c) => state.Grid[c] switch
    {
        'F' => new(InsideOnF, state),
        'L' => new(InsideOnL, state),
        '|' => new(OutsideLoop, state.AddCount()),
        '.' => new(InsideLoop, state)
    };

    record struct Result(Func<State,Coordinate,Result> Next, State State);
    
    record struct State(FiniteGrid Grid, Coordinate p, HashSet<Coordinate> Loop, int Count)
    {
        public State AddCount() => this with { Count = Count + 1 };
    }

    private static IEnumerable<Coordinate> ConnectedNeighbours(FiniteGrid grid, Coordinate c)
    => grid[c] switch
    {
        '|' => [c.N, c.S],
        '-' => [c.E, c.W],
        'L' => [c.N, c.E],
        'J' => [c.N, c.W],
        'F' => [c.S, c.E],
        '7' => [c.S, c.W],
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
        var aoc = new AoC202310(input);
        Assert.Equal(expected, aoc.Part2());
    }
}
