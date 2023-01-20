namespace AdventOfCode.Year2019.Day13;
public class AoC201913
{
    internal static string[] input = Read.InputLines();

    ImmutableDictionary<long, long> program = input.First().Split(',').Select(long.Parse).Select((n, i) => (n, i: (long)i)).ToImmutableDictionary(x => x.i, x => x.n);

    public object Part1()
    {
        return (from chunk in IntCode.Run(program).Chunked3()
                let x = chunk.a
                let y = chunk.b
                let id = chunk.c
                where id == 2
                select id).Count();
    }
    public object Part2()
    {
        program = program.SetItem(0, 2);
        return "";
    }
}



static class IntCode
{
    internal static IEnumerable<long> Run(ImmutableDictionary<long, long> program, params int[] inputs)
    {
        int index = 0;
        int offset = 0;
        int opcode;
        int nextinput = 0;
        while (true)
        {
            (opcode, var modes) = Decode(program[index]);

            if (opcode == 99)
                break;

            var parameters = opcode switch
            {
                1 or 2 or 7 or 8 => program.GetParameters(index, modes, 3),
                3 or 4 or 9 => program.GetParameters(index, modes, 1),
                5 or 6 => program.GetParameters(index, modes, 2)
            };

            var parameterValues = opcode switch
            {
                1 or 2 or 5 or 6 or 7 or 8 => program.GetValues(offset, parameters, 2),
                4 or 9 => program.GetValues(offset, parameters, 1),
                _ => Array.Empty<long>()
            };

            var parameterCount = parameters.Length;
            var jump = parameterCount + 1;

            switch (opcode)
            {
                case 1:
                    {
                        var result = parameterValues.Sum();
                        program = program.Set(parameters[^1], offset, result);
                    }
                    break;
                case 2:
                    {
                        var result = parameterValues.Product();
                        program = program.Set(parameters[^1], offset, result);
                    }
                    break;
                case 3:
                    {
                        var result = inputs[nextinput++];
                        program = program.Set(parameters[^1], offset, result);
                    }
                    break;
                case 4:
                    {
                        yield return parameterValues[0];
                    }
                    break;
                case 5:
                    {
                        if (parameterValues[0] != 0) jump = (int)parameterValues[1] - index;
                    }
                    break;
                case 6:
                    {
                        if (parameterValues[0] == 0) jump = (int)parameterValues[1] - index;
                    }
                    break;
                case 7:
                    {
                        var result = parameterValues[0] < parameterValues[1] ? 1 : 0;
                        program = program.Set(parameters[^1], offset, result);
                    }
                    break;
                case 8:
                    {
                        var result = parameterValues[0] == parameterValues[1] ? 1 : 0;
                        program = program.Set(parameters[^1], offset, result);
                    }
                    break;
                case 9:
                    {
                        offset += (int)parameterValues[0];
                    }
                    break;
                default:
                    throw new Exception();
            }
            index += jump;
        }
    }

    static Parameter[] GetParameters(this ImmutableDictionary<long, long> program, int index, IEnumerable<Mode> modes, int n)
        => Range(index + 1, n).Select(i => program.ContainsKey(i) ? program[i] : 0).Zip(modes, (l, r) => new Parameter(l, r)).ToArray();

    internal static (int opcode, IReadOnlyCollection<Mode> modes) Decode(long value)
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
    static ImmutableDictionary<long, long> Set(this ImmutableDictionary<long, long> program, Parameter parameter, long relativeBase, long value)
    {
        var index = parameter.mode switch
        {
            Mode.Relative => relativeBase + parameter.index,
            _ => parameter.index
        };

        return program.SetItem(index, value);
    }

    static long GetValue(this ImmutableDictionary<long, long> program, long relativeBase, Parameter parameter)
    {
        (var index, var mode) = parameter;
        return mode switch
        {
            Mode.Immediate => index,
            Mode.Position => program.ContainsKey(index) ? program[index] : 0,
            Mode.Relative => program.ContainsKey(index + relativeBase) ? program[index + relativeBase] : 0,
            _ => throw new NotImplementedException()
        };
    }

    static long[] GetValues(this ImmutableDictionary<long, long> program, int relativeBase, IEnumerable<Parameter> parameters, int n)
        => parameters.Take(n).Select(p => program.GetValue(relativeBase, p)).ToArray();
}
record struct Parameter(long index, Mode mode);
enum Mode
{
    Position = 0,
    Immediate = 1,
    Relative = 2
}
