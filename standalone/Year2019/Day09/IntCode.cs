namespace AdventOfCode.Year2019.Day09;

static class IntCode
{
    internal static IEnumerable<long> Run(ImmutableDictionary<long, long> program, params int[] inputs)
    {
        int index = 0;
        int relativeBase = 0;
        int opcode;
        var inputEnumerator = (inputs as IEnumerable<int>).GetEnumerator();
        do
        {
            (opcode, var modes) = Decode(program[index]);
            long? value = null;
            switch (opcode)
            {
                case 1:
                    {
                        const int parameterCount = 3;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(relativeBase, parameters);
                        var result = a + b;
                        var jump = parameterCount + 1;
                        program = program.SetValue(result, parameters.Last());
                        index += jump;
                    }
                    break;
                case 2:
                    {
                        const int parameterCount = 3;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(relativeBase, parameters);
                        var result = a * b;
                        var jump = parameterCount + 1;
                        program = program.SetValue(result, parameters.Last());
                        index += jump;
                    }
                    break;
                case 3:
                    {
                        if (!inputEnumerator.MoveNext()) throw new InvalidOperationException("no more inputs");
                        const int parameterCount = 1;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        var jump = parameterCount + 1;
                        program = program.SetValue(inputEnumerator.Current, parameters.Last());
                        index += jump;
                    }
                    break;
                case 4:
                    {
                        const int parameterCount = 1;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        var jump = parameterCount + 1;
                        index += jump;
                        value = program.GetValue(relativeBase, parameters.First());
                    }
                    break;
                case 5:
                    {
                        const int parameterCount = 2;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(relativeBase, parameters);
                        index = a == 0 ? index + parameterCount + 1 : (int)b;
                    }
                    break;
                case 6:
                    {
                        const int parameterCount = 2;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(relativeBase, parameters);
                        index = a == 0 ? (int)b : index + parameterCount + 1;
                    }
                    break;
                case 7:
                    {
                        const int parameterCount = 3;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(relativeBase, parameters);
                        var result = a < b ? 1 : 0;
                        var jump = parameterCount + 1;
                        program = program.SetValue(result, parameters.Last());
                        index += jump;
                    }
                    break;
                case 8:
                    {
                        const int parameterCount = 3;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(relativeBase, parameters);
                        var result = a == b ? 1 : 0;
                        var jump = parameterCount + 1;
                        program = program.SetValue(result, parameters.Last());
                        index += jump;
                    }
                    break;
                case 9:
                    {
                        const int parameterCount = 1;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        relativeBase += (int)program.GetValue(relativeBase, parameters.First());
                        var jump = parameterCount + 1;
                        index += jump;
                    }
                    break;
                case 99:
                    break;
                default:
                    throw new Exception();
            }
            if (value.HasValue) yield return value.Value;
        }
        while (opcode != 99);
    }

    static IEnumerable<(long value, Mode mode)> GetParameters(this ImmutableDictionary<long, long> program, int index, IEnumerable<Mode> modes, int n)
        => Range(index+1, n).Select(i => program.ContainsKey(i) ? program[i]: 0).Zip(modes, (l, r) => (value: l, mode: r));

    static (int opcode, IReadOnlyCollection<Mode> modes) Decode(long value)
    {
        Mode[] modes = new Mode[3];
        var opcode = value % 100;
        value /= 100;
        for (int i = 0; i < 3; i++)
        {
            modes[i] = (Mode)(value % 10);
            value /= 10;
        }
        return ((int)opcode, modes);
    }
    static ImmutableDictionary<long, long> SetValue(this ImmutableDictionary<long, long> program, long value, (long index, Mode mode) parameter)
        => program.SetItem(parameter.index, value);
    static long GetValue(this ImmutableDictionary<long, long> program, long relativeBase, (long index, Mode mode) parameter)
    {
        (var index, var mode) = parameter;
        return mode switch
        {
            Mode.Immediate => index,
            Mode.Position => program.ContainsKey(index) ? program[index] : 0,
            Mode.Relative => program.ContainsKey(index+relativeBase) ? program[index + relativeBase] : 0,
            _ => throw new NotImplementedException()
        };
    }

    static (long a, long b) GetValues(this ImmutableDictionary<long, long> program, int relativeBase, IEnumerable<(long, Mode)> parameters)
    {
        var enumerator = parameters.GetEnumerator();
        enumerator.MoveNext();
        var a = program.GetValue(relativeBase, enumerator.Current);
        enumerator.MoveNext();
        var b = program.GetValue(relativeBase, enumerator.Current);
        return (a, b);
    }
}

enum Mode
{
    Position,
    Immediate,
    Relative
}


