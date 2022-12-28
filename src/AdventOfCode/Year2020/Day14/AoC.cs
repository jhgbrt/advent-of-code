using Memory = System.Collections.Immutable.ImmutableDictionary<long, long>;
namespace AdventOfCode.Year2020.Day14;

public class AoC202014
{
    static string[] input = Read.InputLines();

    public long Part1() => Part1Impl.Run(input);

    public object Part2() => Part2Impl.Run(input);
}

static class Part1Impl
{
    public static long Run(string[] input)
    {
        var query = from line in input
                    select Factory.Create(line);

        var memory = query.Aggregate(
            (mask: new Mask(0, 0), memory: Memory.Empty),
            (x, i) => i switch
            {
                Mask m => (m, x.memory),
                WriteMemory m => (x.mask, x.memory.SetItem(m.Address, x.mask.Apply(m.Value))),
                _ => x
            }).memory;
        return memory.Sum(m => m.Value);

    }

    internal record Mask(long All, long Ones)
    {
        internal long Apply(long input) => (All & input) | Ones;
    }
    record MaskInput(string Value)
    {
        internal Mask ToBitMask() => new Mask(
            Convert.ToInt64(Value.Replace('X', '1'), 2),
            Convert.ToInt64(Value.Replace('X', '0'), 2)
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
        public static object Create(string line)
            => line[1] switch { 'a' => MaskInput.Parse(line).ToBitMask(), 'e' => WriteMemory.Parse(line), _ => throw new() };
    }

}

static class Part2Impl
{
    public static long Run(string[] input)
    {
        var query = from line in input
                    select Factory.Create(line);

        var memory = query.Aggregate((mask: Mask.Empty, memory: Memory.Empty), (t, item) => item switch
        {
            Mask m => (m, t.memory),
            WriteMemory m => (t.mask, Write(t.memory, t.mask, m)),
            _ => throw new()
        }).memory;

        return memory.Sum(x => x.Value);

    }

    static Memory Write(Memory memory, Mask mask, WriteMemory instruction)
    {
        var addresses = mask.GetAddresses(instruction.Address);
        foreach (var a in addresses)
        {
            memory = memory.SetItem(a, instruction.Value);
        }
        return memory;
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
        public static MaskInput Parse(string line) => _regex.As<MaskInput>(line);
    }

    record struct WriteMemory(int Address, long Value)
    {
        static Regex _regex = new Regex(@"mem\[(?<Address>\d+)\] = (?<Value>\d+)");
        public static WriteMemory Parse(string line) => _regex.As<WriteMemory>(line);
    }

    static class Factory
    {
        public static object Create(string line) => line[1] switch { 'a' => MaskInput.Parse(line).ToBitMask(), 'e' => WriteMemory.Parse(line), _ => throw new() };
       
    }

}

