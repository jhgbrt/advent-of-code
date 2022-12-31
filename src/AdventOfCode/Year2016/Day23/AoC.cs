using AdventOfCode.Year2020.Day10;

namespace AdventOfCode.Year2016.Day23;
public class AoC201623
{
    static string[] input = Read.InputLines();

    static IEnumerable<(string instruction, string first, string second)> instructions
        = from line in input
          let split = line.Split(' ')
          let instruction = split[0]
          let first = split[1]
          let second = split.Length > 2 ? split[2] : string.Empty
          select (instruction, first, second);
    public object Part1() => new Computer().Compute(instructions.ToImmutableArray(), 7);
    public object Part2() => Computer.Hardcoded(7,0,0,0);
}


class Computer
{
    Dictionary<char, int> memory = new()
    {
        ['a'] = 0,
        ['b'] = 0,
        ['c'] = 0,
        ['d'] = 0
    };
    int GetValue(string input) => input[0] switch
    {
        'a' or 'b' or 'c' or 'd' => memory[input[0]],
        _ => int.Parse(input)
    };
    void SetValue(char register, int value)
    {
        if (memory.ContainsKey(register))
            memory[register] = value;
    }


    public static (int a, int b, int c, int d) Hardcoded(int a, int b, int c, int d)
    {
        b = a;
        b--;
        d = a;
        a = 0;
        c = b;
        a++;
        c--;
        while (c != 0)
        {
            c--;
            a++;
        }
        d--;
        while (d != 0)
        {
            a = 0;
            c = b;
            d--;
            c++;
            d--;
        }
        b--;
        c = b;
        d = c;
        d--;
        c++;
        while (d != 0)
        {
            c--;
            d--;
        }
        c = -16;
        while (c != 0)
        {
            c--;
            a++;
        }
        c = 80;
        while (c != 77)
        {
            d--;
            a++;
        }
        a++;
        d--;
        while (d != 0)
        {
            a++;
            d--;
        }
        c--;
        while (c != 0)
        {
            a++;
            c--;
        }
        return (a, b, c, d);
    }


    public int Compute(ImmutableArray<(string instruction, string first, string second)> instructions, int init)
    {
        memory['a'] = init;
        int steps = 0;
        var i = 0;
        while (i < instructions.Length)
        {
            if (i % 10000 == 0) Console.Write(".");
            (var instruction, var first, var second) = instructions[i];
            var before = (memory['a'], memory['b'], memory['c'], memory['d']);
            switch (instruction)
            {
                case "cpy" when first.Length == 1 && char.IsLetter(first[0]):
                    SetValue(second[0], GetValue(first));
                    i++;
                    break;
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
                case "tgl":
                    instructions = Toggle(instructions, i, GetValue(first));
                    i++;
                    break;
                case "mul":
                    SetValue(first[0], GetValue(first) * GetValue(second));
                    i++;
                    break;
                default:
                    i++;
                    break;

            }
            steps++;
            //if (instruction == "inc")
            //    Console.WriteLine($"{(instruction, first, second)}, ({before}) -> ({(memory['a'], memory['b'], memory['c'], memory['d'])}");
            //Console.ReadKey();
        }
        return memory['a'];

    }

    ImmutableArray<(string instruction, string first, string second)> Toggle(ImmutableArray<(string instruction, string first, string second)> instructions, int i, int value)
    {
        var j = i + value;
        if (j < 0 || j >= instructions.Length)
            return instructions;
        
        var current = instructions[j];

        var toggled = current switch
        {
            { instruction: "inc", second: "" } => ("dec", current.first, ""),
            { second: "" } => ("inc", current.first, ""),
            { instruction: "jnz", second: not "" } => ("cpy", current.first, current.second),
            { second: not "" } => ("jnz", current.first, current.second)
        };
        Console.WriteLine($"{current} -> {toggled}");
        return instructions.SetItem(j, toggled);
    }
}

