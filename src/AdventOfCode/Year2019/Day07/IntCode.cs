namespace AdventOfCode.Year2019.Day07;

static class IntCode
{
    internal static IEnumerable<int> Run(ImmutableArray<int> program, params int[] inputs)
    {
        int index = 0;
        int opcode;
        var inputEnumerator = (inputs as IEnumerable<int>).GetEnumerator();
        do
        {
            (opcode, var modes) = Decode(program[index]);
            int? value = null;
            switch (opcode)
            {
                case 1:
                    {
                        const int parameterCount = 3;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(parameters);
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
                        (var a, var b) = program.GetValues(parameters);
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
                        value = program.GetValue(parameters.First());
                    }
                    break;
                case 5:
                    {
                        const int parameterCount = 2;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(parameters);
                        index = a == 0 ? index + parameterCount + 1 : b;
                    }
                    break;
                case 6:
                    {
                        const int parameterCount = 2;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(parameters);
                        index = a == 0 ? b : index + parameterCount + 1;
                    }
                    break;
                case 7:
                    {
                        const int parameterCount = 3;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(parameters);
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
                        (var a, var b) = program.GetValues(parameters);
                        var result = a == b ? 1 : 0;
                        var jump = parameterCount + 1;
                        program = program.SetValue(result, parameters.Last());
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

    static IEnumerable<(int value, Mode mode)> GetParameters(this IEnumerable<int> program, int index, IEnumerable<Mode> modes, int n)
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
    static ImmutableArray<int> SetValue(this ImmutableArray<int> program, int value, (int index, Mode mode) parameter)
        => program.SetItem(parameter.index, value);
    static int GetValue(this ImmutableArray<int> program, (int index, Mode mode) parameter)
    {
        (var index, var mode) = parameter;
        return mode switch
        {
            Mode.Immediate => index,
            Mode.Position => program[index],
            _ => throw new NotImplementedException()
        };
    }

    static (int a, int b) GetValues(this ImmutableArray<int> program, IEnumerable<(int, Mode)> parameters)
    {
        var enumerator = parameters.GetEnumerator();
        enumerator.MoveNext();
        var a = program.GetValue(enumerator.Current);
        enumerator.MoveNext();
        var b = program.GetValue(enumerator.Current);
        return (a, b);
    }
}

enum Mode
{
    Position,
    Immediate
}


