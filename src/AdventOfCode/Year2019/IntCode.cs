using Newtonsoft.Json.Linq;

namespace AdventOfCode.Year2019;


class IntCode
{
    TextWriter? output;
    public bool Halted { get; private set; }
    public long LastOutput { get; private set; }
    private Dictionary<int, long> program;

    public IntCode(IEnumerable<int> program, TextWriter? output = null) : this(program.Select(i=>(long)i), output)
    {
    }
    public IntCode(IEnumerable<long> program, TextWriter? output = null)
    {
        this.program = program.Select((p,i) => (p,i)).ToDictionary(p => p.i, p => p.p);
        this.output = output;
    }
    public long this[int position]
    {
        get => program.TryGetValue(position, out var value) ? value : 0;
        set => program[position] = value;
    }

    public IEnumerable<long> Run(IEnumerable<long>? input = null)
    {
        input ??= Empty<long>();
        var inputEnumerator = input.GetEnumerator();
        while (!Halted)
        {
            var returnValue = RunImpl(inputEnumerator);
            if (returnValue.HasValue)
            {
                output?.WriteLine(returnValue);
                yield return returnValue!.Value;
            }
        }
    }
    public long? Run(long input) => RunImpl(input.AsEnumerable().GetEnumerator());



    int index = 0;
    int offset = 0;
    long? RunImpl(IEnumerator<long> input)
    {       
        long? returnValue = null;
        while (!returnValue.HasValue)
        {

            var intvalue = int.CreateChecked(this[index]);
            var opcode = intvalue % 100;
            var a = intvalue / 100 % 10;
            var b = intvalue / 1000 % 10;
            var c = intvalue / 10000 % 10;
     
            var (nofparams, nofargs, description) = opcode switch
            {
                1 => (3, 2, "add"),
                2 => (3, 2, "multiply"),
                3 => (1, 0, "input"),
                4 => (1, 1, "output"),
                5 => (2, 2, "jump-if-true"),
                6 => (2, 2, "jump-if-false"),
                7 => (3, 2, "less-than"),
                8 => (3, 2, "equals"),
                9 => (1, 1, "adjust-offset"),
                99 => (0, 0, "halt")
            };

            var parameters = new Parameters(
                nofparams < 1 ? default : new(this[index + 1], a), 
                nofparams < 2 ? default : new(this[index + 2], b), 
                nofparams < 3 ? default : new(this[index + 3], c), 
                nofparams);

            var args = (
                a: nofargs < 1 ? 0L : GetValue(parameters.a),
                b: nofargs < 2 ? 0L : GetValue(parameters.b)
            );
            
            output?.WriteLine($"{description}: this[{index}] = {intvalue}; // {opcode}/{a}-{b}-{c}/np={nofparams},na={nofargs}/values={string.Join(",", args)}");
            long? @null = null;
            (bool halt, long? value, int delta, int jump, returnValue) = opcode switch
            {
                1 => (false, args.a + args.b, 0, nofparams + 1, @null),
                2 => (false, args.a * args.b, 0, nofparams + 1, @null),
                3 => input.MoveNext() ? (false, input.Current, 0, nofparams + 1, @null) : throw new Exception("No more input"),
                4 => (false, @null, 0, nofparams + 1, args.a),
                5 => (false, @null, 0, args.a != 0 ? int.CreateChecked(args.b) - index : nofparams + 1, @null),
                6 => (false, @null, 0, args.a == 0 ? int.CreateChecked(args.b) - index : nofparams + 1, @null),
                7 => (false, args.a < args.b ? 1L : 0L, 0, nofparams + 1, @null),
                8 => (false, args.a == args.b ? 1L : 0L, 0, nofparams + 1, @null),
                9 => (false, @null, int.CreateChecked(args.a), nofparams + 1, @null),
                99 => (true, @null, 0, 0, @null)
            };

            if (returnValue.HasValue)
            {
                output?.WriteLine($"output: {returnValue}");
                LastOutput = returnValue.Value;
            }
            if (halt)
            {
                output?.WriteLine("halt");
                Halted = true;
                break;
            }

            if (value.HasValue)
            {
                output?.WriteLine($"this[{GetIndex(parameters.Last)}] = {value.Value}");
                this[GetIndex(parameters.Last)] = value.Value;
            }

            if (delta > 0)
            {
                output?.WriteLine($"offset += {delta} = {offset + delta}");
            }
            offset += delta;

            output?.WriteLine($"index += {jump} = {index + jump}");
            index += jump;
        }
        return returnValue;

        long GetValue(Parameter p) => p.mode switch
        {
            1 => p.value,
            0 or 2 => this[GetIndex(p)]
        };
        int GetIndex(Parameter p) => p.mode switch
        {
            0 => int.CreateChecked(p.value),
            2 => int.CreateChecked(p.value) + offset
        };
    }

    internal readonly record struct Parameters(Parameter a, Parameter b, Parameter c, int n)
    {
        public Parameter Last => n switch { 1 => a, 2 => b, 3 => c };
    }
    internal readonly record struct Parameter(long value, int mode)
    {
        public override string ToString() => mode switch
        {
            0 => $"[{value}]",
            1 => $"{value}",
            2 => $"[{value} + offset]"
        };
    }
   
}

public class IntCodeSpecs
{
    TextWriter writer;

    public IntCodeSpecs(ITestOutputHelper output)
    {
        this.writer = new TestWriter(output);
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
    public void Tests(string program, int input, params int[] expectedOutput)
    {
        var sut = new IntCode(program.Split(',').Select(long.Parse).ToArray(), writer);
        var result = sut.Run([input]);
        Assert.Equal(expectedOutput.Select(i => (long)i), result);
    }
    [Theory]
    [InlineData(10, 7, 1, 2, 3, 7, 99)]   // [7] = [2] + [3] = 3 + 7 = 10
    [InlineData(9, 7, 101, 2, 3, 7, 99)]  // [7] =  2  + [3] =  2 + 7 = 9
    [InlineData(6, 7, 1001, 2, 3, 7, 99)] // [7] = [2] +  3  =  3 + 3 = 6
    [InlineData(5, 7, 1101, 2, 3, 7, 99)] // [7] =  2  +  3  = 5
    public void Opcode1_Add(long expected, int position, params int[] program)
    {
        var sut = new IntCode(program, writer);
        var result = sut.Run().ToList();
        Assert.Equal(expected, sut[position]);
    }
    [Theory]
    [InlineData(21, 7, 0002, 2, 3, 7, 99)] // [7] = [2] * [3] = 3 * 7 = 21
    [InlineData(14, 7, 0102, 2, 3, 7, 99)] // [7] =  2  * [3] = 2 * 7 = 14
    [InlineData(09, 7, 1002, 2, 3, 7, 99)] // [7] = [2] *  3  = 3 * 3 = 9
    [InlineData(06, 7, 1102, 2, 3, 7, 99)] // [7] =  2  *  3  = 6
    public void Opcode2_Multiply(long expected, int position, params int[] program)
    {
        var sut = new IntCode(program, writer);
        var result = sut.Run().ToList();
        Assert.Equal(expected, sut[position]);
    }

    [Theory]
    [InlineData(654, 123, 003, 123, 99)]
    public void Opcode3_Store_Input(int input, int position, params int[] program)
    {
        var sut = new IntCode(program, writer);
        var result = sut.Run([input]).ToList();
        Assert.Equal(input, sut[position]);
    }

    [Theory]
    [InlineData(654, 004, 3, 99, 654)] // 004 = output value at position 3 (= 654)
    [InlineData(3, 104, 3, 99, 654)] // 104 = output value 3
    public void Opcode4_Output(int expected, params int[] program)
    {
        var sut = new IntCode(program, writer);
        var result = sut.Run().ToList();
        Assert.Equal(expected, result.Single());
    }

    // Opcode 5 is jump-if-true: if the first parameter is non-zero, it sets the instruction pointer to the value from the second parameter. Otherwise, it does nothing.

    [Theory]
    [InlineData(654, 1105, 0, 6, 104, 654, 99, 104, 655, 99, 0, 1)]  // 1105 = jump to position 6, first param is 0 => 104 654 = output 654
    [InlineData(655, 1105, 1, 6, 104, 654, 99, 104, 655, 99, 0, 1)]  // 1105 = jump to position 6, first param is 1 => jump to 6 => 104 655 = output 655
    [InlineData(654, 1005, 9, 6, 104, 654, 99, 104, 655, 99, 0, 1)]  // 1005 = jump to position 6, first param at [9] is 0 => 104 654 = output 654
    [InlineData(655, 1005, 0, 6, 104, 654, 99, 104, 655, 99, 0, 1)]  // 1005 = jump to position 6, first param at [0] is not 0 => jump to 6 => 104 655 = output 655
    public void Opcode5_JumpIfTrue(int expected, params int[] program)
    {
        var sut = new IntCode(program, writer);
        var result = sut.Run().ToList();
        Assert.Equal(expected, result.Single());
    }

    // Opcode 6 is jump-if-false: if the first parameter is zero, it sets the instruction pointer to the value from the second parameter. Otherwise, it does nothing.
    [Theory]
    [InlineData(654, 1106, 1, 6, 104, 654, 99, 104, 655, 99, 0, 1)]  // 1105 = jump to position 6, first param is 0 => 104 654 = output 654
    [InlineData(655, 1106, 0, 6, 104, 654, 99, 104, 655, 99, 0, 1)]  // 1105 = jump to position 6, first param is 1 => jump to 6 => 104 655 = output 655
    [InlineData(654, 1006, 0, 6, 104, 654, 99, 104, 655, 99, 0, 1)]  // 1005 = jump to position 6, first param at [9] is 0 => 104 654 = output 654
    [InlineData(655, 1006, 9, 6, 104, 654, 99, 104, 655, 99, 0, 1)]  // 1005 = jump to position 6, first param at [0] is not 0 => jump to 6 => 104 655 = output 655
    public void Opcode6_JumpIfFalse(int expected, params int[] program)
    {
        var sut = new IntCode(program, writer);
        var result = sut.Run().ToList();
        Assert.Equal(expected, result.Single());
    }

    // Opcode 7 is less than: if the first parameter is less than the second parameter, it stores 1 in the position given by the third parameter. Otherwise, it stores 0.
    [Theory]
    [InlineData(1, 123, 1107, 1, 2, 123, 99)]
    [InlineData(0, 123, 1107, 2, 1, 123, 99)]
    [InlineData(1, 123, 0107, 1, 5, 123, 99, 2)]
    [InlineData(0, 123, 1007, 5, 1, 123, 99, 2)]
    public void Opcode7_LessThan(int expected, int position, params int[] program)
    {
        var sut = new IntCode(program, writer);
        var result = sut.Run().ToList();
        Assert.Equal(expected, sut[position]);
    }

    // Opcode 8 is equals: if the first parameter is equal to the second parameter, it stores 1 in the position given by the third parameter. Otherwise, it stores 0
    [Theory]
    [InlineData(1, 123, 1108, 1, 1, 123, 99)]
    [InlineData(0, 123, 1108, 1, 2, 123, 99)]
    [InlineData(1, 123, 0108, 1, 5, 123, 99, 1)]
    [InlineData(0, 123, 1008, 5, 1, 123, 99, 2)]
    public void Opcode_Equals(int expected, int position, params int[] program)
    {
        var sut = new IntCode(program, writer);
        var result = sut.Run().ToList();
        Assert.Equal(expected, sut[position]);
    }
    [Fact]
    public void EmptyProgram()
    {
        var sut = new IntCode([99L], writer);
        var result = sut.Run().ToList();
        Assert.Empty(result);
    }

    [Fact]
    public void ShouldCopyItSelf()
    {
        var program = new[] { 109L, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 }; ;
        var result = new IntCode(program).Run();
        Assert.Equal(program, result);
    }

    [Fact]
    public void ShouldHave16Digits()
    {
        var program = new[] { 1102L, 34915192, 34915192, 7, 4, 7, 99, 0 };
        var result = new IntCode(program).Run();
        Assert.Equal(16, result.First().ToString().Length);
    }

    [Fact]
    public void ShouldOutputLargeInput()
    {
        var program = new[] { 104, 1125899906842624, 99 };
        var result = new IntCode(program).Run();
        Assert.Equal(1125899906842624, result.First());
    }

}