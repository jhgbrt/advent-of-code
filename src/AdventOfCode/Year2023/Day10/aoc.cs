using AdventOfCode.Common;

namespace AdventOfCode.Year2023.Day10;
public class AoC202310
{
    static bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] realinput = Read.InputLines();
    static string[] input = usesample ? sample : realinput;
    public object Part1()
    {
        var grid = new FiniteGrid(input);

        var edges = from p in grid.InteriorPoints()
                    from q in ConnectedNeighbours(grid, p)
                    select (p, q);

        var graph = new Graph();
        foreach (var e in edges)
        {
            graph.AddEdge(new Edge())
        }

        var s = grid.Find('S');

        int step = 0;

        var n = Next(grid,s);
        while (n != s)
        {
            n = Next(grid, n);
            step++;
        }

        Console.WriteLine(s);

        return step;
    }
    public object Part2() => "";


    private Coordinate Next(FiniteGrid grid, Coordinate c) => ConnectedNeighbours(grid, c).First();

    private IEnumerable<Coordinate> ConnectedNeighbours(FiniteGrid grid, Coordinate coordinate)
    => from d in new[] { Direction.N, Direction.E, Direction.S, Direction.W }
       let n = grid.GetNeighbour(coordinate, d)
       where n is not null
       where (d, grid[n.Value]) switch
       {
           (Direction.N, '|' or 'F' or '7') => true,
           (Direction.E, '-' or 'F' or 'L') => true,
           (Direction.S, '|' or 'L' or 'J') => true,
           (Direction.W, '-' or 'J' or '7') => true,
           _ => false
       }
       select n.Value;

}


