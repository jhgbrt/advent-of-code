namespace AdventOfCode.Year2018.Day15;

public class AoC201815
{
    static string[] input = Read.InputLines().TakeWhile(s => !string.IsNullOrEmpty(s)).ToArray();
    static string[] grid = input.Select(l => l.Replace('G', '.').Replace('E', '.')).ToArray();
    static ImmutableList<Unit> Units(int power) => (
            from y in Enumerable.Range(0, input.Length)
            from x in Enumerable.Range(0, input[y].Length)
            let c = input[y][x]
            where c is 'G' or 'E'
            select c is 'G' ? new Goblin((x, y)) as Unit : new Elve((x, y), power)
        ).ToImmutableList();



    public object Part1()
    {
        return Run(Units(3), false)!.Value;
    }
    public object Part2()
    {
        for (int attackPower = 4; ; attackPower++)
        {
            int? outcome = Run(Units(attackPower), true);
            if (outcome.HasValue)
            {
                return outcome;
            }
        }
    }
    static int? Run(ImmutableList<Unit> units, bool part2)
    {

        for (int rounds = 0; ; rounds++)
        {
            units = (from u in units orderby u.Coordinate.y, u.Coordinate.x select u).ToImmutableList();
            for (int i = 0; i < units.Count; i++)
            {
                var unit = units[i];
                var targets = units.Where(t => t is Elve != unit is Elve).ToList();

                if (!targets.Any())
                    return rounds * units.Sum(ru => ru.Health);

                if (!targets.Any(unit.IsNeighbour))
                    unit = Move(unit, targets, units);

                var target = (
                    from t in targets.Where(unit.IsNeighbour)
                    orderby t.Health, t.Coordinate.y, t.Coordinate.x
                    select t
                    ).FirstOrDefault();

                if (target is null)
                    continue;

                target = target.AttackBy(unit);
                if (target.Health > 0)
                    continue;

                if (part2 && target is Elve)
                    return null;

                int index = units.IndexOf(target);
                units = units.RemoveAt(index);
                if (index < i)
                    i--;
            }
        }
    }

    static IEnumerable<(int x, int y)> Neighbours((int x, int y) coordinate)
    {
        (var x, var y) = coordinate;
        yield return (x, y - 1);
        yield return (x - 1, y);
        yield return (x + 1, y);
        yield return (x, y + 1);
    }

    static Unit Move(Unit unit, IEnumerable<Unit> targets, IEnumerable<Unit> units)
    {
        var inRange = (
            from target in targets
            from coordinate in Neighbours(target.Coordinate)
            where IsOpen(coordinate, units)
            select coordinate
            ).Distinct().ToImmutableArray();

        var queue = new Queue<(int x, int y)>();
        var connections = new Dictionary<(int x, int y), (int x, int y)>();
        queue.Enqueue(unit.Coordinate);
        connections.Add(unit.Coordinate, (-1, -1));
        while (queue.Count > 0)
        {
            var coordinate = queue.Dequeue();
            foreach (var neighbour in Neighbours(coordinate).Where(n => !connections.ContainsKey(n) && IsOpen(n, units)))
            {
                queue.Enqueue(neighbour);
                connections.Add(neighbour, coordinate);
            }
        }

        var paths = (from t in inRange
                     let path = GetPath(unit.Coordinate, t, connections)
                     where path.Any()
                     orderby path.Count, t.y, t.x
                     select path).ToList();

        if (paths.Any())
            unit = unit.MoveTo(paths.First().First());

        return unit;
    }
    static IReadOnlyCollection<(int x, int y)> GetPath((int x, int y) source, (int x, int y) destination, IReadOnlyDictionary<(int, int), (int, int)> connections)
    {
        if (!connections.ContainsKey(destination))
            return new List<(int, int)>();
        var path = new List<(int x, int y)>();
        while (destination != source)
        {
            path.Add(destination);
            destination = connections[destination];
        }

        path.Reverse();
        return path;
    }

    static bool IsOpen((int x, int y) coordinate, IEnumerable<Unit> units) => grid[coordinate.y][coordinate.x] == '.' && units.All(u => u.Coordinate != coordinate);
}