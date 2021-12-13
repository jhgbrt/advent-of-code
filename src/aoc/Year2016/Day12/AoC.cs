namespace AdventOfCode.Year2016.Day12;

public class AoC201612 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC201612));
    static IEnumerable<(string instruction, string first, string second)> instructions = from line in input
                                                                                         let split = line.Split(' ')
                                                                                         let instruction = split[0]
                                                                                         let first = split[1]
                                                                                         let second = split.Length > 2 ? split[2] : string.Empty
                                                                                         select (instruction, first, second);

    public override object Part1() => new Computer().Compute(instructions.ToImmutableArray(), 0);

    public override object Part2() => new Computer().Compute(instructions.ToImmutableArray(), 1);
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
        memory[register] = value;
    }

    public int Compute(ImmutableArray<(string instruction, string first, string second)> instructions, int init)
    {
        memory['c'] = init;
        int steps = 0;
        var i = 0;
        while (i < instructions.Length)
        {
            (var instruction, var first, var second) = instructions[i];
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
                    i += int.Parse(second);
                    break;
                default:
                    i++;
                    break;

            }
            steps++;
        }
        return memory['a'];

    }
}

