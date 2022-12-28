namespace AdventOfCode.Year2019.Day07;

class Amplifier
{
    public int[] program;
    int index = 0;
    bool halted = false;
    public bool Halted => halted;
    public int Output { get; private set; }
    Queue<int> inputs = new Queue<int>();
    public Amplifier(IEnumerable<int> program)
    {
        this.program = program.ToArray();
    }
    internal IEnumerable<int?> Run(params int[] input)
    {
        int opcode;
        foreach (var i in input)
            inputs.Enqueue(i);
        do
        {
            (opcode, var modes) = Decode(program[index]);
            switch (opcode)
            {
                case 1:
                    {
                        const int parameterCount = 3;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        var result = a + b;
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = result;
                        index += jump;
                    }
                    break;
                case 2:
                    {
                        const int parameterCount = 3;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        var result = a * b;
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = result;
                        index += jump;
                    }
                    break;
                case 3:
                    {
                        if (!inputs.TryDequeue(out int inputvalue))
                            throw new InvalidOperationException("no more inputs");
                        const int parameterCount = 1;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = inputvalue;
                        index += jump;
                    }
                    break;
                case 4:
                    {
                        const int parameterCount = 1;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        var jump = parameterCount + 1;
                        index += jump;
                        Output = GetValue(program, parameters.First());
                        yield return Output;
                    }
                    break;
                case 5:
                    {
                        const int parameterCount = 2;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        index = a == 0 ? index + parameterCount + 1 : b;
                    }
                    break;
                case 6:
                    {
                        const int parameterCount = 2;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        index = a == 0 ? b : index + parameterCount + 1;
                    }
                    break;
                case 7:
                    {
                        const int parameterCount = 3;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        var result = a < b ? 1 : 0;
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = result;
                        index += jump;
                    }
                    break;
                case 8:
                    {
                        const int parameterCount = 3;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        var result = a == b ? 1 : 0;
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = result;
                        index += jump;
                    }
                    break;
                case 99:
                    halted = true;
                    break;
                default:
                    throw new Exception();
            }
        }
        while (opcode != 99);
        halted = true;
    }

    static IEnumerable<(int value, Mode mode)> GetParameters(IEnumerable<int> program, int index, IEnumerable<Mode> modes, int n)
        => program.Skip(index + 1).Take(n).Zip(modes, (l, r) => (value: l, mode: r));

    static (int opcode, IReadOnlyCollection<Mode> modes) Decode(int value)
    {
        Mode[] modes = new Mode[3];
        var opcode = value % 100;
        value /= 100;
        for (int i = 0; i < 3; i++)
        {
            modes[i] = (Mode)(value % 10);
            value /= 10;
        }
        return (opcode, modes);
    }
    static int GetValue(int[] program, (int index, Mode mode) parameter)
    {
        (var index, var mode) = parameter;
        return mode switch
        {
            Mode.Immediate => index,
            Mode.Position => program[index],
            _ => throw new NotImplementedException()
        };
    }

    static (int a, int b) GetValues(int[] program, IEnumerable<(int, Mode)> parameters)
    {
        var enumerator = parameters.GetEnumerator();
        enumerator.MoveNext();
        var a = GetValue(program, enumerator.Current);
        enumerator.MoveNext();
        var b = GetValue(program, enumerator.Current);
        return (a, b);
    }
}

