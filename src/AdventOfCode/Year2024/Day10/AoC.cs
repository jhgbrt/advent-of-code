namespace AdventOfCode.Year2024.Day10;
using Maze = IReadOnlyDictionary<Coordinate, int>;
public class AoC202410(string[] input)
{
    public AoC202410() : this(Read.InputLines()) { }

    readonly Maze maze = (
        from il in input.Index()
        let y = il.Index
        let line = il.Item
        from ic in line.Index()
        let x = ic.Index
        let c = ic.Item
        where c != '.'
        select KeyValuePair.Create(new Coordinate(x, y), c - '0')
    ).ToDictionary();

    public int Part1() => (
        from c in maze.Keys
        where maze[c] == 0
        select GetScore1(maze, c)
    ).Sum();

    public int Part2() => (
        from c in maze.Keys
        where maze[c] == 0
        select GetScore2(maze, c)
    ).Sum();

    internal int GetScore1(Maze maze, Coordinate start)  
    {
        HashSet<Coordinate> peaks = [];
        Queue<Coordinate> queue = [];
        HashSet<Coordinate> visited = [start];

        queue.Enqueue(start);
        while (queue.Any())
        {
            var current = queue.Dequeue();
            var height = maze[current];
            if (height == 9)
            {
                peaks.Add(current);
                continue;
            }
            foreach (var n in current.Neighbours().Where(c => maze.TryGetValue(c, out var value) && value == height +  1))
            {
                if (!visited.Contains(n))
                {
                    visited.Add(n);
                    queue.Enqueue(n);
                }
            }
        }

        return peaks.Count;
    }

    internal int GetScore2(Maze maze, Coordinate start)
    {
        var stack = new Stack<Coordinate>();
        stack.Push(start);
        var visited = new HashSet<Coordinate>();
        var count = 0;
        while (stack.Any())
        {
            var current = stack.Pop();
            var height = maze[current];
            if (height == 9)
            {
                count++;
                continue;
            }
            visited.Clear();
            foreach (var n in current.Neighbours().Where(c => maze.TryGetValue(c, out var value) && value == height + 1))
            {
                if (!visited.Contains(n))
                {
                    visited.Add(n);
                    stack.Push(n);
                }
            }
        }
        return count;
    }
}
public class AoC202410Tests
{


    [Theory]
    [InlineData(1, 2)]
    [InlineData(2, 4)]
    [InlineData(3, 3)]
    public void TestPart1(int sample, int expected)
    {
        var input = Read.SampleLines(sample);
        var sut = new AoC202410(input);
        Assert.Equal(expected, sut.Part1());
    }

    [Theory]
    [InlineData(4, 3)]
    [InlineData(5, 13)]
    [InlineData(6, 227)]
    [InlineData(7, 81)]
    public void TestPart2(int sample, int expected)
    {
        var input = Read.SampleLines(sample);
        var sut = new AoC202410(input);
        Assert.Equal(expected, sut.Part2());
    }
}

readonly record struct Coordinate(int x, int y)
{
    public override string ToString() => $"({x},{y})";

    public static Coordinate operator +(Coordinate left, (int dx, int dy) p) => new(left.x + p.dx, left.y + p.dy);
    static readonly (int dx, int dy)[] deltas = [ (-1, 0), (0, 1), (1, 0), (0, -1) ];
    public IEnumerable<Coordinate> Neighbours()
    {
        var c = this;
        return from d in deltas
               select c + d;
    }
}
