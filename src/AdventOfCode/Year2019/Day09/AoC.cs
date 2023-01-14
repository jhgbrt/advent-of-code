namespace AdventOfCode.Year2019.Day09;

public class AoC201909
{
    internal static string[] input = Read.InputLines();

    ImmutableDictionary<long, long> program = input.First().Split(',').Select(long.Parse).Select((n, i) => (n, i: (long)i)).ToImmutableDictionary(x => x.i, x => x.n);

    public object Part1() => IntCode.Run(program, 1).Last();

    public object Part2() => IntCode.Run(program, 2).Last();

    [Fact]
    public void ShouldCopyItSelf()
    {
        var program = new[] { 109L, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 }.Select((value,index) => (value,index:(long)index)).ToImmutableDictionary(x=>x.index, x => x.value);
        var result = IntCode.Run(program);
        Assert.Equal(program.Values, result);
    }

    [Fact]
    public void ShouldHave16Digits()
    {
        var program = new[] { 1102L, 34915192, 34915192, 7, 4, 7, 99, 0 }.Select((value, index) => (value, index: (long)index)).ToImmutableDictionary(x => x.index, x => x.value);
        var result = IntCode.Run(program);
        Assert.Equal(16, result.First().ToString().Length);
    }
    [Fact]
    public void ShouldOutputLargeInput()
    {
        var program = new[] { 104, 1125899906842624, 99 }.Select((value, index) => (value, index: (long)index)).ToImmutableDictionary(x => x.index, x => x.value);
        var result = IntCode.Run(program);
        Assert.Equal(1125899906842624, result.First());
    }
    [Fact]
    public void Malfunction()
    {
        var program = new[] {9, 13, 203L, 50, 4, 50, 4, 63, 99}.Select((value, index) => (value, index: (long)index)).ToImmutableDictionary(x => x.index, x => x.value);
        var result = IntCode.Run(program, 17);
        Assert.Equal(new[] { 0, 17L }, result.ToArray());
    }
    [Fact]
    public void DecodeTests()
    {
        var (opcode, modes) = IntCode.Decode(203);
        Assert.Equal(3, opcode);
        Assert.Equal(new[] { Mode.Relative, Mode.Position, Mode.Position }, modes);

        (opcode, modes) = IntCode.Decode(2003);
        Assert.Equal(3, opcode);
        Assert.Equal(new[] { Mode.Position, Mode.Relative, Mode.Position }, modes);

        (opcode, modes) = IntCode.Decode(20003);
        Assert.Equal(3, opcode);
        Assert.Equal(new[] { Mode.Position, Mode.Position, Mode.Relative}, modes);
    }
}

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
            switch (opcode)
            {
                case 1:
                    {
                        const int parameterCount = 3;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        (var a, var b) = program.GetValues(relativeBase, parameters);
                        var result = a + b;
                        var jump = parameterCount + 1;
                        program = program.SetValue(relativeBase, result, parameters.Last());
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
                        program = program.SetValue(relativeBase, result, parameters.Last());
                        index += jump;
                    }
                    break;
                case 3:
                    {
                        if (!inputEnumerator.MoveNext()) throw new InvalidOperationException("no more inputs");
                        const int parameterCount = 1;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        var jump = parameterCount + 1;
                        program = program.SetValue(relativeBase, inputEnumerator.Current, parameters.Last());
                        index += jump;
                    }
                    break;
                case 4:
                    {
                        const int parameterCount = 1;
                        var parameters = program.GetParameters(index, modes, parameterCount);
                        var jump = parameterCount + 1;
                        index += jump;
                        var value = program.GetValue(relativeBase, parameters.First());
                        yield return value;
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
                        program = program.SetValue(relativeBase, result, parameters.Last());
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
                        program = program.SetValue(relativeBase, result, parameters.Last());
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
        }
        while (opcode != 99);
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
    static ImmutableDictionary<long, long> SetValue(this ImmutableDictionary<long, long> program, long relativeBase, long value, Parameter parameter)
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

    static (long a, long b) GetValues(this ImmutableDictionary<long, long> program, int relativeBase, Parameter[] parameters)
    {
        var a = program.GetValue(relativeBase, parameters[0]);
        var b = program.GetValue(relativeBase, parameters[1]);
        return (a, b);
    }
}
record struct Parameter(long index, Mode mode);
enum Mode
{
    Position = 0,
    Immediate = 1,
    Relative = 2
}


