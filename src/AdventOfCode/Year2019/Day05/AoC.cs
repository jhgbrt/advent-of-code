using AdventOfCode.Year2019.Day07;

using Sprache;

using System.Collections;

namespace AdventOfCode.Year2019.Day05;

public class AoC201905
{
    int[] program;
    public AoC201905() : this(Read.InputLines()) { }
    public AoC201905(string[] input)
    {
        program = input[0].Split(',').Select(int.Parse).ToArray();
    }
 
    public object Part1() => Run(1).Last();
    public object Part2() => Run(5).Last();
    public IEnumerable<int> Run(int input) => new IntCode(program).Run(input);
  

}

public class Specs
{
    private readonly ITestOutputHelper output;
    public Specs(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [InlineData("1,9,10,3,2,3,11,0,99,30,40,50", 1)]
    [InlineData("1,0,0,0,99", 1)]
    [InlineData("2,3,0,3,99", 1)]
    [InlineData("2,4,4,5,99,0", 1)]
    [InlineData("1,1,1,4,99,5,6,0,99", 1)]
    [InlineData("1101,100,101,5,104,0,99", 1, 201)]
    [InlineData("1101,0,0,5,4,0,99", 1, 1101)]
    [InlineData("1102,100,101,5,104,0,99", 1, 10100)]
    [InlineData("2,0,0,5,4,0,99", 1, 4)]
    [InlineData("3,0,4,0,99", 123, 123)]
    [InlineData("4,0,99", 1, 4)]
    [InlineData("1105,0,6,4,0,99,4,3,99", 1, 1105)]
    [InlineData("1105,1,6,4,0,99,4,3,99", 1, 4)]
    [InlineData("1106,1,6,4,0,99,4,3,99", 1, 1106)]
    [InlineData("1106,0,6,4,0,99,4,3,99", 1, 4)]
    [InlineData("3,9,8,9,10,9,4,9,99,-1,8", 7, 0)]
    [InlineData("3,9,8,9,10,9,4,9,99,-1,8", 8, 1)]
    [InlineData("3,9,8,9,10,9,4,9,99,-1,8", 9, 0)]
    [InlineData("3,9,7,9,10,9,4,9,99,-1,8", 7, 1)]
    [InlineData("3,9,7,9,10,9,4,9,99,-1,8", 8, 0)]
    [InlineData("3,9,7,9,10,9,4,9,99,-1,8", 9, 0)]
    [InlineData("3,3,1108,-1,8,3,4,3,99", 7, 0)]
    [InlineData("3,3,1108,-1,8,3,4,3,99", 8, 1)]
    [InlineData("3,3,1108,-1,8,3,4,3,99", 9, 0)]
    [InlineData("3,3,1107,-1,8,3,4,3,99", 7, 1)]
    [InlineData("3,3,1107,-1,8,3,4,3,99", 8, 0)]
    [InlineData("3,3,1107,-1,8,3,4,3,99", 9, 0)]
    [InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9", 0, 0)]
    [InlineData("3,12,6,12,15,1,13,14,13,4,13,99,-1,0,1,9", 9, 1)]
    [InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1", 0, 0)]
    [InlineData("3,3,1105,-1,9,1101,0,0,12,4,12,99,1", 9, 1)]
    [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 0, 999)]
    [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 7, 999)]
    [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 8, 1000)]
    [InlineData("3,21,1008,21,8,20,1005,20,22,107,8,21,20,1006,20,31,1106,0,36,98,0,0,1002,21,125,20,4,20,1105,1,46,104,999,1105,1,46,1101,1000,1,20,4,20,1105,1,46,98,99", 9, 1001)]
    public void TestPart1(string program, int input, params int[] expectedOutput)
    {
        var sut = new AoC201905([program]);
        var result = sut.Run(input);
        Assert.Equal(expectedOutput, result);
    }

    [Fact]
    public void Part1()
    {
        Assert.Equal(4887191, new AoC201905(Read.SampleLines()).Part1());
    }
    [Fact]
    public void Part2()
    {
        Assert.Equal(3419022, new AoC201905(Read.SampleLines()).Part2());
    }

    [Theory]
    [InlineData(1, 1, 0, 0, 0)]
    [InlineData(199, 99, 1, 0, 0)]
    [InlineData(1199, 99, 1, 1, 0)]
    [InlineData(11143, 43, 1, 1, 1)]
    public void DecodeTest(int value, int expectedOpCode, params int[] expectedModes)
    {
        IntCode.Mode[] expected = expectedModes.Select(i => (IntCode.Mode)i).ToArray();
        (var opcode, var modes) = IntCode.Decode(value);
        Assert.Equal(expectedOpCode, opcode);
        Assert.Equal(expected, modes);
    }

}


public class IntCode
{
    TextWriter? output;
    public bool IsTerminated { get; private set; }
    private Dictionary<int, int> program;

    Parameter[] parametersBuffer = new Parameter[3];
    Mode[] modes = new Mode[3];
    int[] values = new int[3];

    public IntCode(int[] program, TextWriter? output = null)
    {
        this.program = Range(0, program.Length).ToDictionary(i => i, i => program[i]);
        this.output = output;
    }

    internal IEnumerable<int> Run(int input)
    {
        int index = 0;
        int opcode;
        Mode[] modes;
        do
        {
            (opcode, modes) = Decode(program[index]);
            int? value = null;
            switch (opcode)
            {
                case 1:
                {
                    const int parameterCount = 3;
                    var parameters = GetParameters(index, modes, parameterCount);
                    (var a, var b) = GetValues(parameters, 0, 2).Chunked2().First();
                    var result = a + b;
                    var jump = parameterCount + 1;
                    SetValue(parameters.Last(), 0, result);
                    index += jump;
                }
                break;
                case 2:
                {
                    const int parameterCount = 3;
                    var parameters = GetParameters(index, modes, parameterCount);
                    (var a, var b) = GetValues(parameters, 0, 2).Chunked2().First();
                    var result = a * b;
                    var jump = parameterCount + 1;
                    SetValue(parameters.Last(), 0, result);
                    index += jump;
                }
                break;
                case 3:
                {
                    const int parameterCount = 1;
                    var parameters = GetParameters(index, modes, parameterCount);
                    var jump = parameterCount + 1;
                    SetValue(parameters.Last(), 0, input);
                    index += jump;
                }
                break;
                case 4:
                {
                    const int parameterCount = 1;
                    var parameters = GetParameters(index, modes, parameterCount);
                    var jump = parameterCount + 1;
                    index += jump;
                    value = GetValue(parameters.First(), 0);
                }
                break;
                case 5:
                {
                    const int parameterCount = 2;
                    var parameters = GetParameters(index, modes, parameterCount);
                    (var a, var b) = GetValues(parameters, 0, 2).Chunked2().First();
                    index = (int)(a == 0 ? index + parameterCount + 1 : b);
                }
                break;
                case 6:
                {
                    const int parameterCount = 2;
                    var parameters = GetParameters(index, modes, parameterCount);
                    (var a, var b) = GetValues(parameters, 0, 2).Chunked2().First();
                    index = (int) (a == 0 ? b : index + parameterCount + 1);
                }
                break;
                case 7:
                {
                    const int parameterCount = 3;
                    var parameters = GetParameters(index, modes, parameterCount);
                    (var a, var b) = GetValues(parameters, 0, 2).Chunked2().First();
                    var result = a < b ? 1 : 0;
                    var jump = parameterCount + 1;
                    SetValue(parameters.Last(), 0, result);
                    index += jump;
                }
                break;
                case 8:
                {
                    const int parameterCount = 3;
                    var parameters = GetParameters(index, modes, parameterCount);
                    (var a, var b) = GetValues(parameters, 0, 2).Chunked2().First();
                    var result = a == b ? 1 : 0;
                    var jump = parameterCount + 1;
                    SetValue(parameters.Last(), 0, result);
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


    internal static (int opcode, Mode[] modes) Decode(int value)
    {
        Mode[] modes = new Mode[3];
        var opcode = value % 100;
        value /= 100;
        for (long i = 0; i < 3; i++)
        {
            modes[i] = (Mode)(value % 10);
            value /= 10;
        }
        return (opcode, modes);
    }
    ManagedBuffer<Parameter> GetParameters(int index, Mode[] modes, int n)
    {
        for (int i = 0; i < n; i++)
        {
            var m = modes[i];
            var value = program.ContainsKey(index + i + 1) ? program[index + i + 1] : 0;
            parametersBuffer[i] = new(value, m);
        }
        return Managed(parametersBuffer, n);
    }

    void SetValue(Parameter parameter, int offset, int value)
    {
        var (index, _) = parameter.Get(offset);
        program[index] = value;
    }

    int GetValue(Parameter parameter, int offset)
    {
        var (index, value) = parameter.Get(offset);
        return parameter.mode switch
        {
            Mode.Immediate => value,
            _ => program.ContainsKey(index) ? program[index] : 0
        };
    }
    ManagedBuffer<int> GetValues(ManagedBuffer<Parameter> parameters, int offset, int n)
    {
        for (int i = 0; i < n; i++)
        {
            values[i] = GetValue(parameters[i], offset);
        }
        return Managed(values, n);
    }

    static ManagedBuffer<T> Managed<T>(T[] array, int count) => new(array, count);
    struct ManagedBuffer<T>(T[] array, int count) : IEnumerable<T>
    {
        public T this[int index] => index >= 0 && index < count ? array[index] : throw new IndexOutOfRangeException();
        public int Count => count;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    readonly record struct Parameter(int value, Mode mode)
    {
        public (int index, int value) Get(int offset) => mode switch
        {
            Mode.Relative => (int.CreateChecked(value) + offset, -1),
            Mode.Position => (int.CreateChecked(value), -1),
            Mode.Immediate => (-1, value)
        };
        public int Value => value;
    }
    internal enum Mode
    {
        Position = 0,
        Immediate = 1,
        Relative = 2
    }
}


