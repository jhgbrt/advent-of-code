using System.Collections;

namespace AdventOfCode.Year2021.Day23;

public class AoC202123
{
    static string[] input = Read.InputLines();
    static GameState initial = GameState.Parse(input);
    static GameState goal = new GameState(Hall.Empty, Rooms.Create(new[] { "AA", "BB", "CC", "DD" }));
    static GameState goal2 = new GameState(Hall.Empty, Rooms.Create(new[] { "AAAA", "BBBB", "CCCC", "DDDD" }));

    public object Part1() => Dijkstra(initial, goal);

    public object Part2() => Dijkstra(initial.Part2(), goal2);

    int Dijkstra(GameState source, GameState target)
    {
        var costs = new Dictionary<GameState, int>() { [source] = 0 };
        var visited = new HashSet<GameState>();
        var queue = new PriorityQueue<GameState, int>();

        queue.Enqueue(source, 0);

        while (queue.Count > 0)
        {
            var state = queue.Dequeue();
            if (!visited.Contains(state))
            {
                visited.Add(state);
                if (state == target) break;
                var updates = from move in state.PossibleMoves()
                              let cost = costs[state] + move.Cost
                              where cost < costs.GetValueOrDefault(move.NewState, int.MaxValue)
                              select (move.NewState, cost);

                foreach (var (next, cost) in updates)
                {
                    queue.Enqueue(next, cost);
                    costs[next] = cost;
                    if (next == target) Console.WriteLine((next, cost));
                }
            }
        }

        return costs[target];
    }



}
record GameState(Hall Hall, Rooms Rooms)
{
    public override string ToString()
    {
        return $"Hall = {Hall}, Rooms = {Rooms.A.Value},{Rooms.B.Value},{Rooms.C.Value},{Rooms.B.Value}";
    }
    public static GameState Parse(string[] input)
    {
        var rooms = Rooms.Create(
            from line in input
            let entries = line.Where(char.IsLetter).Select((c, i) => (c, i))
            where entries.Count() == 4
            from entry in entries
            group entry.c by entry.i into g
            select string.Join("", g)
            );
        return new GameState(Hall.Empty, rooms);
    }
    public GameState Part2() => this with
    {
        Rooms = Rooms.Create(new[]
            {
                Rooms.A.Value.Insert(1, "DD"),
                Rooms.B.Value.Insert(1, "CB"),
                Rooms.C.Value.Insert(1, "BA"),
                Rooms.D.Value.Insert(1, "AC"),
            })
    };

    private static Amphipod[] Amphipods = Range(0, 4).Select(x => new Amphipod((char)('A' + x), (int)Math.Pow(10, x))).ToArray();
    private static int GetEnergyFactor(char c) => Amphipods[c - 'A'].Energy;

    public IEnumerable<Move> PossibleMoves()
        => (from i in new[] {'A','B','C','D'}
            from pos in AllOpenSpaces(i)
            let move = TryMoveOut(i, pos)
            where move.HasValue
            select move.Value
            ).Concat(
            from i in Range(0, Hall.Length)
            let move = TryMoveIn(i)
            where move.HasValue
            select move.Value);

    private Move? TryMoveOut(char roomID, int target)
    {
        var room = Rooms[roomID];
        if (room.IsEmpty) return null;

        var depth = room.Depth;

        var steps = Math.Abs(target - Rooms[roomID].Position) + depth + 1;
        var amphipod = room[depth];

        var hall = new Hall(Hall.Value.Remove(target, 1).Insert(target, amphipod.ToString()));

        room = room.Clear(depth);

        var updatedRooms = Rooms.SetItem(roomID, room);

        return new Move(
            this with { Hall = hall, Rooms = updatedRooms },
            steps * GetEnergyFactor(amphipod)
            );
    }

    private Move? TryMoveIn(int hallwayPosition)
    {
        var amphipod = Hall[hallwayPosition];
        if (amphipod == '.') return null;

        var room = Rooms[amphipod];
        var target = room.Position;
        var start = target > hallwayPosition ? hallwayPosition + 1 : hallwayPosition - 1;
        var min = Math.Min(target, start);
        var max = Math.Max(target, start);
        if (Hall.Value[min..max].Any(ch => ch != '.'))
            return null;

        if (!room.CanEnter(amphipod))
            return null;

        var depth = room.LastEmptyPosition;
        var steps = max - min + 1 + depth + 1;

        var hall = new Hall(Hall.Value.Remove(hallwayPosition, 1).Insert(hallwayPosition, "."));

        return new Move(this with 
        { 
            Hall = hall, 
            Rooms = Rooms.SetItem(amphipod, room.SetItem(depth, amphipod)) 
        }, steps * GetEnergyFactor(amphipod));
    }

    private IEnumerable<int> AllOpenSpaces(char roomIndex)
    {
        var position = Rooms[roomIndex].Position;

        for (var i = position - 1; i >= 0 && Hall[i] == '.'; --i)
        {
            if (!Rooms.IsRoomPosition(i))
                yield return i;
        }

        for (var i = position + 1; i < Hall.Length && Hall[i] == '.'; ++i)
        {
            if (!Rooms.IsRoomPosition(i))
                yield return i;
        }
    }

}
record struct Amphipod(char Id, int Energy);

record struct Rooms(Room A, Room B, Room C, Room D)
{
    public static Rooms Create(IEnumerable<string> values)
    {
        var v = values.Select((v, i) => new Room(v, (i+1)*2)).ToArray();
        return new Rooms(v[0], v[1], v[2], v[3]);
    }
    IEnumerable<Room> All() { yield return A; yield return B; yield return C; yield return D; } 
    public bool IsRoomPosition(int i) => All().Any(r => r.Position == i);

    internal Rooms SetItem(char index, Room room)
    {
        return index switch {
            'A' => this with { A = room },
            'B' => this with { B = room },
            'C' => this with { C = room },
            'D' => this with { D = room },
            _ => throw new NotImplementedException()
        };
    }

    internal Room this[char index] => index switch
    {
        'A'=>A,'B'=>B,'C'=>C,'D'=>D,_=>throw new NotImplementedException()
    };
}
record struct Hall(string Value)
{
    public readonly static Hall Empty = new Hall("...........");
    public char this[int i] => Value[i];
    public int Length => Value.Length;
}
record struct Room(string Value, int Position)
{
    public bool IsEmpty => Value.All(c => c == '.');

    public int LastEmptyPosition => Value.Select((c, i) => (c, i)).Last(c => c.c == '.').i;

    public int Depth => Value.IndexOfAny(new[] { 'A', 'B', 'C', 'D' });

    public char this[int depth] => Value[depth];
    internal Room Clear(int depth) => this with { Value = Value.Remove(depth, 1).Insert(depth, ".") };

    internal bool CanEnter(char amphipod) => Value.All(ch => ch == '.' || ch == amphipod);

    internal Room SetItem(int depth, char amphipod) => this with { Value = Value.Remove(depth, 1).Insert(depth, amphipod.ToString()) };
}
record struct Move(GameState NewState, int Cost);


