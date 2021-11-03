﻿using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Memory = System.Collections.Immutable.ImmutableDictionary<int, long>;
namespace Part1;

public static class Part1
{
    public static void Run()
    {
        var input = File.ReadLines(@"..\input.txt");

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
        Console.WriteLine(memory.Sum(m => m.Value));

    }

}


internal record Mask(long All, long Ones)
{
    internal long Apply(long input) => (All & input) | Ones;
}
partial record MaskInput(string Value)
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
