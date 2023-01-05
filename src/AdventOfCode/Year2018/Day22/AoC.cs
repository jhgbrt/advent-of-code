using AdventOfCode.Year2018.Day04;
using AdventOfCode.Year2022.Day22;

using Net.Code.AdventOfCode.Toolkit.Core;

namespace AdventOfCode.Year2018.Day22;

public class AoC201822
{

    const int targetx = 10;
    const int targety = 785;

    public int Part1() => Region.All().Sum(r => r.RiskLevel);
    public int Part2()
    {
        State start = new State(Region.GetRegion(new(0, 0)), Tool.Torch);
        State target = new State(Region.GetRegion(new(targetx, targety)), Tool.Torch);
        var queue = new Queue<(State, int switching, int minutes)>();
        var visited = new HashSet<State>();
        queue.Enqueue((start, 0, 0));
        visited.Add(start);
        while (queue.Count > 0)
        {
            (State state, int switching, int minutes) = queue.Dequeue();
            var (region, tool) = state;
            if (switching > 1 || visited.Add(state))
                queue.Enqueue((state, switching - 1, minutes + 1));

            if (switching > 0)
                continue;

            if (state == target)
                return minutes;


            var neighbours = from n in region.Neighbours()
                             let s = state with { region = n }
                             where n.IsAllowed(tool) && visited.Add(s)
                             select s;

            foreach (var s in neighbours)
                queue.Enqueue((s, 0, minutes + 1));

            var other = region.GetOtherTool(tool);
            queue.Enqueue((state with { tool = other }, 7, minutes));
        }

        return -1;
    }

}

readonly record struct State(Region region, Tool tool);

readonly record struct Region(Point Position, RegionType Type)
{
    const int depth = 5616;
    const int targetx = 10;
    const int targety = 785;

    public static IEnumerable<Region> All()
    {
        for (int x = 0; x <= targetx; x++)
            for (int y = 0; y <= targety; y++)
                yield return GetRegion(new(x,y));
    }
    
    public int RiskLevel => (int)Type;

    public static Region GetRegion(Point p) => new Region(p, (RegionType)(ErosionLevel(p) % 3));


    static int GeologicIndex(Point p) => p switch
    {
        (0, 0) => 0,
        (targetx, targety) => 0,
        (_, 0) => p.x * 16807,
        (0, _) => p.y * 48271,
        _ => ErosionLevel(new(p.x - 1, p.y)) * ErosionLevel(new(p.x, p.y - 1))
    };

    static Dictionary<Point, int> _cache = new();
    static private int ErosionLevel(Point p)
    {
        if (_cache.ContainsKey(p)) return _cache[p];
        var result = (GeologicIndex(p) + depth) % 20183;
        _cache[p] = result;
        return result;
    }

    public bool IsAllowed(Tool tool) => (Type, tool) switch
    {
        (RegionType.Rocky, Tool.Climbing or Tool.Torch) => true,
        (RegionType.Wet, Tool.Climbing or Tool.Neither) => true,
        (RegionType.Narrow, Tool.Torch or Tool.Neither) => true,
        _ => false
    };
    public Tool GetOtherTool(Tool tool) => (Type, tool) switch
    {
        (RegionType.Rocky, Tool.Climbing) => Tool.Torch,
        (RegionType.Rocky, Tool.Torch) => Tool.Climbing,
        (RegionType.Wet, Tool.Climbing) => Tool.Neither,
        (RegionType.Wet, Tool.Neither) => Tool.Climbing,
        (RegionType.Narrow, Tool.Torch) => Tool.Neither,
        (RegionType.Narrow, Tool.Neither) => Tool.Torch
    };

    public IEnumerable<Region> Neighbours() => Position.Neighbours().Select(GetRegion);
}

readonly record struct Point(int x, int y)
{
    internal IEnumerable<Point> Neighbours()
    {
        if (x > 0) yield return new Point(x - 1, y);
        yield return new Point(x, y + 1);
        if (y > 0) yield return new Point(x, y - 1);
        yield return new Point(x + 1, y);
    }
}

enum RegionType
{
    Rocky = 0,
    Wet = 1,
    Narrow = 2
}

enum Tool
{
    Torch,
    Climbing,
    Neither
}
