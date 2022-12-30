namespace AdventOfCode.Year2016.Day22;
public class AoC201622
{
    static readonly bool interactive = false;
    static readonly string[] input = Read.InputLines();
    
    static readonly Regex regex = new Regex(@"/dev/grid/node-x(?<x>\d+)-y(?<y>\d+) +\d+T +(?<used>\d+)T +(?<avail>\d+)T +\d+%");
    
    static ImmutableList<Node> nodes = (
        from line in input.Skip(2)
        let data = regex.As<Data>(line)
        let c = new Coordinate(data.x, data.y)
        select new Node(c, data.used, data.avail)
        ).ToImmutableList();

    public object Part1() => (from a in nodes
                              from b in nodes
                              where a.c != b.c && a.used > 0 && b.avail >= a.used
                              select (a, b)).Count();
    public object Part2()
    {
        var origin = new Coordinate(0, 0);
        var empty = (from node in nodes where node.used == 0 select node.c).Single();
        var walls = (from node in nodes where node.used > 100 select node.c).ToHashSet();
        var goal = new Coordinate(36, 0);

        int steps = 0;

        while (empty.x > 0)
        {
            steps++;
            empty = empty with { x = empty.x - 1 };
            Write(empty, goal, walls, steps);
        }
        while (empty.y > 0)
        {
            steps++;
            empty = empty with { y = empty.y - 1 };
            Write(empty, goal, walls, steps);
        }
        while (empty.x < goal.x - 1)
        {
            steps++;
            empty = empty with { x = empty.x + 1 };
            Write(empty, goal, walls, steps);
        }
        (goal, empty) = (empty, goal);
        steps += 1;
        Write(empty, goal, walls, steps);
        while (goal != origin)
        {
            steps++;
            empty = empty with { y = empty.y + 1 };
            Write(empty, goal, walls, steps);

            for (int i = 0; i < 2; i++)
            {
                steps++;
                empty = empty with { x = empty.x - 1 };
                Write(empty, goal, walls, steps);
            }
            steps++;
            empty = empty with { y = empty.y - 1 };
            Write(empty, goal, walls, steps);
            (goal, empty) = (empty, goal);
            steps++;
            Write(empty, goal, walls, steps);
        }

        return steps;
    }

    void Write(Coordinate empty, Coordinate goal, HashSet<Coordinate> walls, int steps)
    {
        if (!interactive)
            return;
        var sb = new StringBuilder();
        for (int y = 0; y < 10; y++)
        {
            for (int x = 0; x < 37; x++)
            {
                var c = new Coordinate(x, y);
                if (c == empty)
                    sb.Append('_');
                else if (c == goal)
                    sb.Append('G');
                else if (walls.Contains(c))
                    sb.Append('#');
                else
                    sb.Append('.');
            }
            sb.AppendLine();
        }
        Console.Clear();
        Console.WriteLine(steps);
        Console.WriteLine(sb.ToString());
        Console.ReadKey();
    }
}

readonly record struct Data(int x, int y, int used, int avail);
readonly record struct Node(Coordinate c, int used, int avail);
readonly record struct Coordinate(int x, int y);
