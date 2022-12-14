using AdventOfCode.Year2018.Day23;

namespace AdventOfCode.Year2022.Day14;

class Grid
{
    private Dictionary<(int x, int y), char> _grid;

    public int MaxY;
    public int MinX => _grid.Keys.Min(x => x.x);
    public int MaxX => _grid.Keys.Max(x => x.x);
    private (int x, int y) Source = (500, 0);

    public Grid(IEnumerable<(int x, int y)> tiles)
    {
        _grid = new();
        MaxY = tiles.Max(x => x.y);

        foreach (var pos in tiles)
            this[pos] = '#';
        this[Source] = '+';
    }

    public int Simulate()
    {
        int steps = 0;
        while (Step())
        {
            steps++;
        }
        return steps;
    }

    public int Simulate2()
    {
        int steps = 0;
        while (Step2())
        {
            Console.WriteLine(this);
            Console.WriteLine("any key to continue");
            steps++;
        }
        return steps;
    }

    bool Step()
    {
        var position = Source;

        while (position.y < MaxY && CanMove(position))
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

        if (position.y < MaxY)
        {
            this[position] = 'o';
            return true;
        }

        return false;
    }
    bool Step2()
    {
        var position = Source;

        while (position.y < MaxY && CanMove(position))
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
        return position != Source;
    }


    bool CanMove((int x, int y) pos) => CanMoveDown(pos) || CanMoveDownLeft(pos) || CanMoveDownRight(pos);
    bool CanMoveDown((int x, int y) pos) =>  this[pos.Down()] == '.';
    bool CanMoveDownLeft((int x, int y) pos) => this[pos.Left().Down()] == '.';
    bool CanMoveDownRight((int x, int y) pos) => this[pos.Right().Down()] == '.';

    char this[(int x, int y) c]
    {
        get => _grid.ContainsKey(c) ? _grid[c] : '.';
        set { _grid[c] = value; }
    }

    public static Grid Parse(IEnumerable<string> input)
    {
        var coordinates = from line in input
                          from coordinate in ParseLine(line)
                          select coordinate;
        return new Grid(coordinates);
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