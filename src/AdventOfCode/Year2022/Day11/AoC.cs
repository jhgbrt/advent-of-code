using System.Numerics;

namespace AdventOfCode.Year2022.Day11;
public class AoC202211
{
    static string[] input = Read.InputLines();

    public ulong Part1()
    {
        var monkeys = input.Chunk(7).Select(Monkey.Parse).ToImmutableArray();
            
        foreach (var round in Range(1, 20))
        {
            monkeys = DoRound(monkeys);
        }

        return monkeys
            .OrderByDescending(m => m.Inspections)
            .Take(2)
            .Aggregate(1ul, (a, x) => a*x.Inspections);
    }

    public ulong Part2()
    {
        var monkeys = input.Chunk(7).Select(Monkey.Parse).ToImmutableArray();
        var reduction = monkeys.Aggregate(1ul, (a, m) => a * m.Test);
        foreach (var round in Range(1, 10000))
        {
            monkeys = DoRound(monkeys, reduction);
        }

        return monkeys
            .OrderByDescending(m => m.Inspections)
            .Take(2)
            .Aggregate(1ul, (a, x) => a * x.Inspections);
    }

    static ImmutableArray<Monkey> DoRound(ImmutableArray<Monkey> monkeys, ulong? reducer = null)
    {
        var builder = monkeys.ToBuilder();
        foreach (var monkey in monkeys)
        {
            ulong inspections = (ulong)monkey.Items.Count;
            foreach (var item in monkey.Items)
            {
                var newitem = monkey.Inspect(item);
                if (!reducer.HasValue)
                    newitem /= 3;
                else
                {
                    while (newitem > reducer.Value)
                        newitem %= reducer.Value;
                }
                var nextmonkey =
                    newitem % monkey.Test == 0
                    ? monkey.IfTrue
                    : monkey.IfFalse;
                builder[nextmonkey].Items.Add(newitem);
            }
            monkey.Items.Clear();
            builder[monkey.Id] = monkey with { Inspections = monkey.Inspections + inspections};
        }

        return builder.ToImmutable();
    }

}

record Monkey(int  Id, List<ulong> Items, Func<ulong, ulong> Inspect, ulong Test, int IfTrue, int IfFalse, ulong Inspections)
{
    public static Monkey Parse(string[] chunk)
    {
        var id = int.Parse(chunk[0][7..^1]);
        var items = chunk[1][18..].Split(',', StringSplitOptions.TrimEntries).Select(ulong.Parse).ToList();
        Func<ulong, ulong> operation = chunk[2][19..] switch
        {
            "old * old" => x => x * x,
            _ => chunk[2][23] switch
            {
                '+' => x => x + ulong.Parse(chunk[2][25..]),
                '*' => x => x * ulong.Parse(chunk[2][25..]),
                _ => throw new NotSupportedException($"unknown operation: {chunk[2][24]}")
            }
        };
        var test = ulong.Parse(chunk[3][21..]);
        var iftrue = int.Parse(chunk[4][28..]);
        var iffalse = int.Parse(chunk[5][29..]);

        return new Monkey(id, items, operation, test, iftrue, iffalse, 0);
    }

    public override string ToString() => $"id: {Id}, Items: {string.Join(",", Items)}, test: {Test}, true: {IfTrue}, false: {IfFalse}, Inspections: {Inspections}";
}

class Number
{
    List<List<int>> factors;

    public void Add(int x) => factors.Add(new List<int> { x });
    public void Multiply(int x)
    {
        foreach (var f in factors) f.Add(x);
    }
    public void Square()
    {

    }
}