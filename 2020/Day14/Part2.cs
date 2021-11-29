
using Memory = System.Collections.Immutable.ImmutableDictionary<long, long>;
namespace AdventOfCode.Year2020.Day14.Part2;

static class Part2
{
    public static long Run()
    {
        var query = from line in File.ReadLines(@"input.txt")
                    select Factory.Create(line);

        var memory = query.Aggregate((mask: default(Mask), memory: Memory.Empty), (t, item) => item switch
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
    internal IEnumerable<long> GetAddresses(long input)
        => from i in Enumerable.Range(0, (int)Math.Pow(2, Offsets.Length))
           select Offsets.Select((offset, i) => (offset, i)).Aggregate(Ones | input & ~Floating, (bits, p) => bits | 1L << p.offset & (((1L << p.i) & i) >> p.i) << p.offset);
}

record MaskInput(string Value)
{
    internal Mask ToBitMask() => new(
        Value.Select((c, i) => (c, i)).Where(x => x.c == 'X').Select(x => Value.Length - 1 - x.i).ToArray(),
        Convert.ToInt64(Value.Replace('X', '0'), 2),
        Convert.ToInt64(Value.Replace('1', '0').Replace('X', '1'), 2)
        );
    static Regex _regex = new Regex(@"mask = (?<Value>\w+)");
    public static MaskInput Parse(string line)
    {
        var match = _regex.Match(line);
        return new MaskInput(match.Groups["Value"].Value);
    }
}

record WriteMemory(int Address, long Value)
{
    static Regex _regex = new Regex(@"mem\[(?<Address>\d+)\] = (?<Value>\d+)");
    public static WriteMemory Parse(string line)
    {
        var match = _regex.Match(line);
        return new WriteMemory(int.Parse(match.Groups["Address"].Value), long.Parse(match.Groups["Value"].Value));

    }
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
