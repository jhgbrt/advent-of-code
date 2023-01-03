namespace AdventOfCode.Year2016.Day11;

using Building = ImmutableList<Floor>;
public class AoC201611
{
    const int SM = 1;
    const int SG = -1;
    const int PM = 2;
    const int PG = -2;
    const int TM = 3;
    const int TG = -3;
    const int RM = 4;
    const int RG = -4;
    const int CM = 5;
    const int CG = -5;
    const int EM = 7;
    const int EG = -7;
    const int DM = 8;
    const int DG = -8;

    public object Part1()
    {
        Building start = new[]
        {
            Floor.WithItems(SM, SG, PM, PG),
            Floor.WithItems(TG, RM, RG, CM, CG),
            Floor.WithItems(TM),
            Floor.Empty
        }.ToImmutableList();

        var endhash = new[] {
            Floor.Empty,
            Floor.Empty,
            Floor.Empty,
            Floor.WithItems(SM, SG, PM, PG, TG, RM, RG, CM, CG, TM)
        }.ToImmutableList().GetHash(3);

        return Calculate(start, endhash);
    }

    public object Part2()
    {

        Building start = new[]
        {
            Floor.WithItems(SM, SG, PM, PG, EM, EG, DM, DG),
            Floor.WithItems(TG, RM, RG, CM, CG),
            Floor.WithItems(TM),
            Floor.Empty
        }.ToImmutableList();

        var endhash = new[] {
            Floor.Empty,
            Floor.Empty,
            Floor.Empty,
            Floor.WithItems(SM, SG, PM, PG, EM, EG, DM, DG, TG, RM, RG, CM, CG, TM)
        }.ToImmutableList().GetHash(3);

        return Calculate(start, endhash);
    }


    static int Calculate(Building start, string endhash)
    {
        var states = new HashSet<string>();
        var queue = new Queue<(int, Building, int, string)>();
        queue.Enqueue((0, start, 0, start.GetHash(0)));
        while (true)
        {
            var (elevator, floors, moves, currenthash) = queue.Dequeue();

            if (!states.Contains(currenthash))
            {
                states.Add(currenthash);

                if (currenthash == endhash) return moves;

                foreach (var index1 in Range(0, floors[elevator].Count()))
                {
                    foreach (var index2 in Range(index1 + 1, floors[elevator].Count() - index1 - 1))
                    {
                        floors = ExploreState(queue, states, 3, elevator, floors, 1, moves, index1, index2);
                        floors = ExploreState(queue, states, 0, elevator, floors, -1, moves, index1, index2);
                    }
                    floors = ExploreState(queue, states, 3, elevator, floors, 1, moves, index1);
                    floors = ExploreState(queue, states, 0, elevator, floors, -1, moves, index1);
                }
            }

        }
    }


    static Building ExploreState(
        Queue<(int, Building, int, string)> queue,
        HashSet<string> states, 
        int req, int elevator, Building floors, int direction, int moves, int? index1, int? index2 = null)
    {
        if (direction * elevator < req)
        {
            floors = floors
                .Move(elevator, direction, index2, 0)
                .Move(elevator, direction, index1, 0);

            var nexthash = floors.GetHash(elevator + direction);

            if (!states.Contains(nexthash)
                && floors[elevator + direction].Any()
                && floors[elevator + direction].IsValidState())
            {
                queue.Enqueue((elevator + direction, floors, moves + 1, nexthash));
            }

            floors = floors
                .Move(elevator + direction, -direction, 0, index1)
                .Move(elevator + direction, -direction, 0, index2);
        }
        return floors;
    }
     


}

static class BuildingEx
{
    public static Building Move(this Building floors, int elevator, int direction, int? fromIndex, int? toIndex)
    {
        if (fromIndex.HasValue && toIndex.HasValue)
        {
            var item = floors[elevator][fromIndex.Value];
            var floorFrom = floors[elevator].RemoveAt(fromIndex.Value);
            var floorTo = floors[elevator + direction].Insert(toIndex.Value, item);
            floors = floors
                .SetItem(elevator, floorFrom)
                .SetItem(elevator + direction, floorTo);

        }
        return floors;
    }
    public static string GetHash(this Building b, int elevator)
    {
        var q = from f in b
                let len = f.Count()
                let sum = f.Count(x => x < 0)
                select (len, sum);

        return $"{elevator}[{string.Join(",", q)}]";
    }
}


readonly record struct Floor(IReadOnlyList<int> items)
{
    public static readonly Floor Empty = new (Array.Empty<int>());
    public static Floor WithItems(params int[] items) => new Floor(items);
    public int this[int i] => items[i];
    public Floor RemoveAt(int i)
    {
        var copy = items.ToList();
        copy.RemoveAt(i);
        return new Floor(copy);
    }
    public Floor Insert(int i, int item)
    {
        var copy = items.ToList();
        copy.Insert(i, item);
        return new Floor(copy);
    }

    public bool IsValidState()
    {
        var items = this.items;
        var hasGen = items.Any(x => x < 0);
        var unpaired = items.Any(x => x > 0 && !items.Contains(-x));
        return !(hasGen && unpaired);
    }

    public int Count() => items.Count;
    public int Count(Func<int, bool> where) => items.Count(where);
    public bool Any() => items.Count > 0;
    public bool Any(Func<int, bool> where) => items.Any(where);
    
}
