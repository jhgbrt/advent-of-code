namespace AdventOfCode.Year2022.Day14;
public class AoC202214
{
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();

    (int x, int y)[] lines = (from line in input
                              from part in line.Split(" -> ")
                              let coordinates = part.Split(",")
                              let x = int.Parse(coordinates[0])
                              let y = int.Parse(coordinates[1])
                              select (x, y)).ToArray();

    public int Part1() => Grid.Parse(input, 1).DoSimulation();
    public int Part2() => Grid.Parse(input, 2).DoSimulation();
}


class Grid
{
    private Dictionary<(int x, int y), char> _grid;

    public int MaxY;
    public int MinX => _grid.Keys.Min(x => x.x);
    public int MaxX => _grid.Keys.Max(x => x.x);
    private (int x, int y) Source = (500, 0);
    private int part;

    public Grid(IEnumerable<(int x, int y)> tiles, int part)
    {
        this.part = part;
        _grid = new();
        MaxY = tiles.Max(x => x.y) + part - 1;

        foreach (var pos in tiles)
            this[pos] = '#';
        this[Source] = '+';
    }

    public int DoSimulation()
    {
        int steps = 0;
        while (Step())
        {
            steps++;
        }
        return steps - 1 + part;
    }

    bool Step()
    {
        var position = Source;

        while (CanMove(position))
        {
            while (CanMoveDown(position))
            {
                position = position.Down();
            }
            if (CanMoveDownLeft(position))
                position = position.Down().Left();
            else if (CanMoveDownRight(position))
                position = position.Down().Right();
        }

        this[position] = 'o';

        return part switch
        {
            1 => position.y < MaxY,
            2 => position != Source,
            _ => false
        };
    }


    bool CanMove((int x, int y) pos) => CanMoveDown(pos) || CanMoveDownLeft(pos) || CanMoveDownRight(pos);
    bool CanMoveDown((int x, int y) pos) => pos.y < MaxY && this[pos.Down()] == '.';
    bool CanMoveDownLeft((int x, int y) pos) => pos.y < MaxY && this[pos.Left().Down()] == '.';
    bool CanMoveDownRight((int x, int y) pos) => pos.y < MaxY && this[pos.Right().Down()] == '.';

    char this[(int x, int y) c]
    {
        get => _grid.ContainsKey(c) ? _grid[c] : '.';
        set { _grid[c] = value; }
    }

    public static Grid Parse(IEnumerable<string> input, int part)
    {
        var coordinates = from line in input
                          from coordinate in ParseLine(line)
                          select coordinate;
        return new Grid(coordinates, part);
    }
    public static IEnumerable<(int x, int y)> ParseLine(string line)
    {
        var coordinates = (
            from part in line.Split(" -> ")
            let pair = part.Split(',')
            let x = int.Parse(pair[0])
            let y = int.Parse(pair[1])
            select (x, y)
            ).ToArray();

        for (int i = 0; i < coordinates.Length - 1; i++)
        {
            var start = coordinates[i];
            var end = coordinates[i + 1];
            if (start.x == end.x)
            {
                var y1 = Math.Min(start.y, end.y);
                var y2 = Math.Max(start.y, end.y);
                for (var y = y1; y <= y2; y++)
                {
                    yield return (start.x, y);
                }
            }
            if (start.y == end.y)
            {
                var x1 = Math.Min(start.x, end.x);
                var x2 = Math.Max(start.x, end.x);
                for (var x = x1; x <= x2; x++)
                {
                    yield return (x, start.y);
                }
            }
        }

    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int y = 0; y <= MaxY; y++)
        {
            for (int x = MinX; x <= MaxX; x++)
            {
                sb.Append(this[(x, y)]);
            }
            if (y != MaxY)
                sb.AppendLine();
        }
        return sb.ToString();
    }
}
static class Ex
{
    public static (int x, int y) Up(this (int x, int y) c) => (c.x, c.y - 1);
    public static (int x, int y) Down(this (int x, int y) c) => (c.x, c.y + 1);
    public static (int x, int y) Left(this (int x, int y) c) => (c.x - 1, c.y);
    public static (int x, int y) Right(this (int x, int y) c) => (c.x + 1, c.y);
}
