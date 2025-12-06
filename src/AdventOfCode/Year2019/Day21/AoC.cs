namespace AdventOfCode.Year2019.Day21;

using System.Collections.Immutable;

public class AoC201921(string[] input)
{
    public AoC201921() : this(Read.InputLines()) { }

    public object Part1()
    {
        return 0;
    }

    public object Part2()
    {
        return -1;
    }

    IntCode GetIntCode(int? n)
    {
        var program = input![0].Split(',').Select(long.Parse).ToArray();
        if (n.HasValue) program[0] = n.Value;
        var intcode = new IntCode(program);
        return intcode;
    }

}



