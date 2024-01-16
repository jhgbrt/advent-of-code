using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Year2019;


class IntCode
{
    public bool IsTerminated { get; private set; }
    private Dictionary<int, int> program;
    public IntCode(ImmutableDictionary<int, int> program) => this.program = program.ToDictionary();

    public (int a, int b, int c) Step(int input) => (Run(input), Run(input), Run(input));

    public IEnumerable<int> Run()
    {
        while (!IsTerminated)
        {
            yield return Run(0);
        }
    }

    int index = 0;
    int offset = 0;

    public int Run(int input)
    {
        int opcode;
        while (!IsTerminated)
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
                _ => Array.Empty<int>()
            };

            var parameterCount = parameters.Length;
            var jump = parameterCount + 1;

            switch (opcode)
            {
                case 1:
                {
                    var result = parameterValues.Sum();
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 2:
                {
                    var result = parameterValues.Product();
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 3:
                {
                    int result = input;
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 4:
                {
                    index += jump;
                    return parameterValues[0];
                }
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
                    program.Set(parameters[^1], offset, result);
                }
                break;
                case 8:
                {
                    var result = parameterValues[0] == parameterValues[1] ? 1 : 0;
                    program.Set(parameters[^1], offset, result);
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
        IsTerminated = true;
        return int.MinValue;
    }


    internal static (int opcode, IReadOnlyCollection<Mode> modes) Decode(int value)
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
}


static class Ex
{
    internal static Parameter[] GetParameters(this IReadOnlyDictionary<int, int> program, int index, IEnumerable<Mode> modes, int n)
        => Range(index + 1, n).Select(i => program.ContainsKey(i) ? program[i] : 0).Zip(modes, (l, r) => new Parameter(l, r)).ToArray();
    internal static void Set(this Dictionary<int, int> program, Parameter parameter, int offset, int value)
    {
        var index = parameter.mode switch
        {
            Mode.Relative => offset + parameter.index,
            _ => parameter.index
        };

        program[index] = value;
    }

    internal static int GetValue(this IReadOnlyDictionary<int, int> program, int offset, Parameter parameter)
    {
        (var index, var mode) = parameter;
        return mode switch
        {
            Mode.Immediate => index,
            Mode.Position => program.ContainsKey(index) ? program[index] : 0,
            Mode.Relative => program.ContainsKey(index + offset) ? program[index + offset] : 0,
            _ => throw new NotImplementedException()
        };
    }
    internal static int[] GetValues(this IReadOnlyDictionary<int, int> program, int relativeBase, IEnumerable<Parameter> parameters, int n)
        => parameters.Take(n).Select(p => program.GetValue(relativeBase, p)).ToArray();

}

record struct Parameter(int index, Mode mode);
enum Mode
{
    Position = 0,
    Immediate = 1,
    Relative = 2
}
