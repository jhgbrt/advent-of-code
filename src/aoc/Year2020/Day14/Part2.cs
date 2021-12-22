
using Memory = System.Collections.Immutable.ImmutableDictionary<long, long>;
namespace AdventOfCode.Year2020.Day14.Part2;

static class Part2
{
    public static long Run()
    {
        var query = from line in Read.InputLines()
                    select Factory.Create(line);

        var memory = query.Aggregate((mask: Mask.Empty, memory: Memory.Empty), (t, item) => item switch
        {
            Mask m => (m, t.memory),
            WriteMemory m => (t.mask, t.memory.Write(t.mask, m)),
            _ => throw new()
        }).memory;

        return memory.Sum(x => x.Value);

    }
}


internal record Mask(int[] Offsets, long Ones, long Floating)
{
    public static Mask Empty => new(Array.Empty<int>(), 0, 0);
    internal IEnumerable<long> GetAddresses(long input)
        => from i in Enumerable.Range(0, (int)Math.Pow(2, Offsets.Length))
           select Offsets.Select((offset, i) => (offset, i)).Aggregate(Ones | input & ~Floating, (bits, p) => bits | 1L << p.offset & (((1L << p.i) & i) >> p.i) << p.offset);
}

record struct MaskInput(string Value)
{
    internal Mask ToBitMask()
    {
        var v = Value;
        return new(Value.Select((c, i) => (c, i)).Where(x => x.c == 'X').Select(x => v.Length - 1 - x.i).ToArray(),
            Convert.ToInt64(Value.Replace('X', '0'), 2),
            Convert.ToInt64(Value.Replace('1', '0').Replace('X', '1'), 2)
            );
    }

    static Regex _regex = new Regex(@"mask = (?<Value>\w+)");
    public static MaskInput Parse(string line) => _regex.As<MaskInput>(line)!.Value;
}

record struct WriteMemory(int Address, long Value)
{
    static Regex _regex = new Regex(@"mem\[(?<Address>\d+)\] = (?<Value>\d+)");
    public static WriteMemory Parse(string line) => _regex.As<WriteMemory>(line)!.Value;
}

static class Factory
{
    public static object Create(string line) => line[1] switch { 'a' => MaskInput.Parse(line).ToBitMask(), 'e' => WriteMemory.Parse(line), _ => throw new() };
    public static string PrintBinary(this long n) => Convert.ToString(n, 2).PadLeft(36, '0');
    public static Memory Write(this Memory memory, Mask mask, WriteMemory instruction)
    {
        var addresses = mask.GetAddresses(instruction.Address);
        foreach (var a in addresses)
        {
            memory = memory.SetItem(a, instruction.Value);
        }
        return memory;
    }
}
