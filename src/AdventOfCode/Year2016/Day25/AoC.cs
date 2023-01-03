using System;

namespace AdventOfCode.Year2016.Day25;

public class AoC201625
{
    static string[] input = Read.InputLines();

    static ImmutableArray<Instruction> instructions
        = (from line in input
           let split = line.Split(' ')
           let instruction = split[0]
           let first = split[1]
           let second = split.Length > 2 ? split[2] : string.Empty
           select new Instruction(instruction, first, second)).ToImmutableArray();

    public object Part1()
    {
        var computer = new Computer(instructions);
        foreach (var i in Range(1, int.MaxValue))
        {
            if (computer.Compute(i, 0, 0, 0).Take(100).SequenceEqual(Repeat01().Take(100)))
                return i;
        }
        return -1;
    }
    public object Part2() => "";

    static IEnumerable<int> Repeat01()
    {
        while (true)
        {
            yield return 0;
            yield return 1;
        }
    }
}

class Computer
{
    readonly ImmutableArray<Instruction> instructions;
    public Computer(ImmutableArray<Instruction> instructions)
    {
        this.instructions = instructions;
    }
    
    (int a, int b, int c, int d) Memory;

    int GetValue(string input) => input[0] switch
    {
        'a' => Memory.a,
        'b' => Memory.b,
        'c' => Memory.c,
        'd' => Memory.d,
        _ => int.Parse(input)
    };
    void SetValue(char register, int value)
    {
        Memory = register switch
        {
            'a' => Memory with { a = value },
            'b' => Memory with { b = value },
            'c' => Memory with { c = value },
            'd' => Memory with { d = value },
        };
    }
    public IEnumerable<int> Compute(int a, int b, int c, int d)
    {
        Memory = (a, b, c, d);
        int steps = 0;
        var i = 0;
        while (i < instructions.Length)
        {
            (var instruction, var first, var second) = instructions[i];
            switch (instruction)
            {
                case "cpy":
                    SetValue(second[0], GetValue(first));
                    i++;
                    break;
                case "inc":
                    SetValue(first[0], GetValue(first) + 1);
                    i++;
                    break;
                case "dec":
                    SetValue(first[0], GetValue(first) - 1);
                    i++;
                    break;
                case "jnz" when GetValue(first) != 0:
                    i += GetValue(second);
                    break;
                case "out":
                    yield return GetValue(first);
                    i++;
                    break;
                default:
                    i++;
                    break;
            }
            steps++;
        }
    }
  
    public string ToCCode()
    {
        var sb = new StringBuilder();
        sb.AppendLine($$"""
            namespace AdventOfCode.Year2016.Day25.Generated
            {
                public partial class Engine
                {
                    public static IEnumerable<int> Compute(int a, int b, int c, int d)
                    {
            """
        );
        for (int i = 0; i < instructions.Length; i++)
        {
            sb.Append($"__{i}: ");
            sb.Append(instructions[i].ToCCode(i));
            sb.AppendLine();
        }

        sb.AppendLine($$"""
                    }
                }
            }
            """);


        return sb.ToString();
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var i in instructions)
            sb.AppendLine(i.ToString());
        return sb.ToString();
    }
}

internal record struct Instruction(string instruction, string first, string second)
{
    public override string ToString() => $"{instruction} {first} {second}";
    public string ToCCode(int i)
    {
        return instruction switch
        {
            "inc" => $"{first}++;",
            "dec" => $"{first}--;",
            "cpy" => $"{second} = {first};",
            "jnz" when int.TryParse(second, out var v) => $"if ({first} != 0) goto __{i + v};",
            "out" => $"yield return {first};",
            _ => this.ToString()
        };
    }

}