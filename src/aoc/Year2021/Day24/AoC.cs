namespace AdventOfCode.Year2021.Day24;

public class AoC202124
{
    static string[] input = Read.InputLines();

    static ImmutableArray<Instruction> program = input.Select(Parse).ToImmutableArray();

    static (int p1, int p2, int p3)[] parameters = SeqModule.Zip3(
        new[] { 1, 1, 1, 1, 26, 1, 1, 26, 1, 26, 26, 26, 26, 26 },
        new[] { 15, 11, 10, 12, -11, 11, 14, -6, 10, -6, -6, -16, -4, -2 },
        new[] { 9, 1, 11, 3, 10, 5, 0, 7, 9, 15, 4, 10, 4, 9 }
        ).Select(t => (t.Item1, t.Item2, t.Item3)).ToArray();

    IEnumerable<(int digit, int p1, int p2, int p3)> Parameters(FNumber n)
        => from item in n.Digits().Zip(parameters)
           select (item.First, item.Second.p1, item.Second.p2, item.Second.p3);

                                             [Theory]
    [InlineData(0, 0, 0, 0, "inp w", 1, 0, 0, 0, 1)]
    [InlineData(3, 0, 0, 0, "mul w 2", 6, 0, 0, 0)]
    [InlineData(0, 2, 0, 0, "add x 3", 0, 5, 0, 0)]
    [InlineData(0, 0, 14, 0, "div y 7", 0, 0, 2, 0)]
    [InlineData(0, 0, 0, 13, "mod z 10", 0, 0, 0, 3)]
    [InlineData(0, 0, 0, 13, "eql z 13", 0, 0, 0, 1)]
    [InlineData(0, 0, 0, 12, "eql z 13", 0, 0, 0, 0)]
    [InlineData(3, 0, 0, 0, "mul w w", 9, 0, 0, 0)]
    [InlineData(3, 2, 0, 0, "add x w", 3, 5, 0, 0)]
    [InlineData(2, 0, 14, 0, "div y w", 2, 0, 7, 0)]
    [InlineData(10, 0, 0, 13, "mod z w", 10, 0, 0, 3)]
    [InlineData(13, 0, 0, 13, "eql z w", 13, 0, 0, 1)]
    [InlineData(12, 0, 0, 13, "eql z w", 12, 0, 0, 0)]
    [InlineData(1, 1, 0, 0, "mul y 0", 1, 1, 0, 0)]
    public void TestInstruction(int w, int x, int y, int z, string instruction, int ew, int ex, int ey, int ez, params int[] input)
    {
        var memory = new Memory(w, x, y, z);
        var f = Parse(instruction);
        var result = f.F(memory, input.OfType<int>().GetEnumerator());
        Assert.Equal(new Memory(ew, ex, ey, ez), result);
    }

    [Fact]
    public void TestProgram1()
    {
        var program = new[]
        {
            "inp x",
            "mul x -1"
        }.Select(Parse).ToImmutableArray();
        var result = Evaluate(new Memory(), program, Repeat(5, 1).GetEnumerator());
        Assert.Equal(-5, result.x);
    }
    [Fact]
    public void TestProgram2()
    {
        var program = new[]
        {
            "inp w",
            "add z w",
            "mod z 2",
            "div w 2",
            "add y w",
            "mod y 2",
            "div w 2",
            "add x w",
            "mod x 2",
            "div w 2",
            "mod w 2"
        }.Select(Parse).ToImmutableArray();
        var result = Evaluate(new Memory(), program, Repeat(0b1101, 1).GetEnumerator());
        Assert.Equal(new Memory(1, 1, 0, 1), result);
    }

    [Fact]
    public void TestFunction()
    {
        var p1s = new[] { 1, 1, 1, 1, 26, 1, 1, 26, 1, 26, 26, 26, 26, 26 };
        var p2s = new[] { 15, 11, 10, 12, -11, 11, 14, -6, 10, -6, -6, -16, -4, -2 };
        var p3s = new[] { 9, 1, 11, 3, 10, 5, 0, 7, 9, 15, 4, 10, 4, 9 };

        var q = Parameters(new FNumber(12345678912345));

        var z = 0;
        foreach (var (digit, p1, p2, p3) in q)
        {
            z = Calculate(z, digit, p1, p2, p3);
        }
        var memory2 = Evaluate(new Memory(), program, new FNumber(12345678912345).Digits().GetEnumerator());

        Assert.Equal(memory2.z, z);
    }

    static Instruction Parse(string line) => new Instruction(GetFunction(line), line);

    static Func<Memory,IEnumerator<int>,Memory> GetFunction(string line)
    {
        var split = line.Split();
        var register1 = split[1][0];

        if (split.Length == 2)
        {
            return (memory, input) =>
            {
                Console.WriteLine($"[ALU] {memory}");
                if (!input.MoveNext()) throw new InvalidOperationException("no more inputs!");
                var value = input.Current;
                return memory.SetValue(register1, value);
            };
        }
        Debug.Assert(split.Length == 3);

        Func<int, int, int> operation = split[0] switch
        {
            "mul" => (int i, int j) => i * j,
            "add" => (int i, int j) => i + j,
            "div" => (int i, int j) => i / j,
            "mod" => (int i, int j) => i % j,
            "eql" => (int i, int j) => i == j ? 1 : 0,
            _ => throw new NotImplementedException(),
        };

        var arg2 = split[2];
        if (char.IsLetter(arg2[0]))
        {
            var register2 = arg2[0];
            return (memory, _) => memory.SetValue(register1, operation(memory.GetValue(register1), memory.GetValue(register2)));
        }
        else
        {
            var value = int.Parse(arg2);
            return (memory, _) => memory.SetValue(register1, operation(memory.GetValue(register1), value));
        }

    }

    public object Part1()
    {
        foreach (var serialNr in PossibleNumbers())
        {
            int result = 0;
            foreach (var p in Parameters(serialNr))
                result = Calculate(result, p.digit, p.p1, p.p2, p.p3);
            if (result == 0)
                return serialNr.value;
        }
        return 0;
    }

    IEnumerable<FNumber> PossibleNumbers()
    {
        for (var f = FNumber.Max; f.value >= FNumber.Min.value; f = f.Decrement())
        {
            yield return f;
        }
    }


    public object Part2() => -1;
    
    private static Memory Evaluate(Memory memory, ImmutableArray<Instruction> program, IEnumerator<int> input)
    {
        foreach (var instruction in program)
        {
            memory = instruction.F(memory, input);
            Console.WriteLine($"--{memory}");
        }
        return memory;
    }

    static int Calculate(int z, int digit, int p1, int p2, int p3)
    {
        if (p1 == 26)
        {
            var x = z % 26; // 0 - 25
            x = (x + p2) == digit ? 0 : 1;
            z = z / 26 * (25 * x + 1) + x * (digit + p3);
            return z;
        }
        else
        {
            var x = (z % 26 + p2) == digit ? 0 : 1;
            z = z * (25 * x + 1) + x * (digit + p3);
            return z;
        }
    }

}

readonly record struct FNumber(long value)
{
    public static FNumber Min => new FNumber(11111111111111);
    public static FNumber Max => new FNumber(99999999999999);
    public IEnumerable<int> Digits()
    {
        var factor = 10000000000000;
        while (factor >= 1)
        {
            yield return (int)(value / factor % 10);
            factor /= 10;
        }
    }

    public FNumber Increment()
    {
        var v = value;
        int n = 0;
        while (v % 10 == 9)
        {
            n++;
            v /= 10;
        }
        var inc = 1;
        for (int i = 0; i < n; i++)
        {
            inc += (int)Math.Pow(10, i);
        }
        return new FNumber(value + inc);
    }
    public FNumber Decrement()
    {
        var v = value;
        int n = 0;
        while (v % 10 == 1)
        {
            n++;
            v /= 10;
        }
        var dec = 1;
        for (int i = 0; i < n; i++)
        {
            dec += (int)Math.Pow(10, i);
        }
        return new FNumber(value - dec);
    }

}

readonly record struct Memory(int w, int x, int y, int z)
{
    public int GetValue(char register)
    {
        return register switch
        {
            'w' => w,
            'x' => x,
            'y' => y,
            'z' => z,
            _ => throw new NotImplementedException()
        };
    }

    public Memory SetValue(char register, int value)
    {
        return register switch
        {
            'w' => this with { w = value },
            'x' => this with { x = value },
            'y' => this with { y = value },
            'z' => this with { z = value },
            _ => throw new NotImplementedException()
        };
    }

}

record Instruction(Func<Memory, IEnumerator<int>, Memory> F, string Name)
{
}

public class Tests
{
    [Theory]
    [InlineData(11111111111111, 11111111111112)]
    [InlineData(11111111111119, 11111111111121)]
    [InlineData(11111111111199, 11111111111211)]
    [InlineData(11111111111399, 11111111111411)]
    [InlineData(11111111111999, 11111111112111)]
    [InlineData(11111111119999, 11111111121111)]
    public void Increment(long first, long second)
    {
        var f = new FNumber(first);
        Assert.Equal(second, f.Increment().value);
    }
    [Theory]
    [InlineData(11111111111111, 11111111111112)]
    [InlineData(11111111111119, 11111111111121)]
    [InlineData(11111111111199, 11111111111211)]
    [InlineData(11111111111399, 11111111111411)]
    [InlineData(11111111111999, 11111111112111)]
    [InlineData(11111111119999, 11111111121111)]
    public void Decrement(long first, long second)
    {
        var f = new FNumber(second);
        Assert.Equal(first, f.Decrement().value);
    }
    [Fact]
    public void Digits()
    {
        Assert.Equal(14, FNumber.Min.Digits().Count());
        Assert.Equal(14, FNumber.Max.Digits().Count());
        Assert.Equal(new[] {1,2,3,4,5,6,7,8,9,1,2,3,4,5}, new FNumber(12345678912345).Digits().ToArray());
    }

}



/*


inp w     mul x 0   add x z   mod x 26  div z 1   add x 15  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 9   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 1   add x 11  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 1   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 1   add x 10  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 11  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 1   add x 12  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 3   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 26  add x -11 eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 10  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 1   add x 11  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 5   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 1   add x 14  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 0   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 26  add x -6  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 7   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 1   add x 10  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 9   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 26  add x -6  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 15  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 26  add x -6  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 4   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 26  add x -16 eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 10  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 26  add x -4  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 4   mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z 26  add x -2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y 9   mul y x   add z y


foreach (var i in input)
{
    w = i;              // inp w 
    x *= 0;             // mul x 0
    x += z;             // add x z
    x %= 26;            // mod x 26
    z /= p1;            // div z p1  
    x += p2;            // add x p2  
    x = x == w ? 1 : 0; // eql x w   
    x = x == 0 ? 1 : 0; // eql x 0   
    y *= 0;             // mul y 0   
    y += 25;            // add y 25  
    y *= x;             // mul y x   
    y += 1;             // add y 1   
    z *= y;             // mul z y   
    y *= 0;             // mul y 0   
    y += w;             // add y w   
    y += p3;            // add y p3  
    y *= x;             // mul y x   
    z += y;             // add z y       
}


inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
inp w     mul x 0   add x z   mod x 26  div z p1  add x p2  eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y p3  mul y x   add z y   
 
                w      x            y      z
inp w       |   i  |            |      | 
mul x 0     |      |   0        |      | 
add x z     |      |   z        |      | 
mod x 26    |      | z%26       |      | 
div z p1    |      |            |      |  z /= p1
add x p2    |      | z%26+p2    |      | 
eql x w     |   i  |            |      | 
eql x 0     |      |            |      | 
mul y 0     |      |            |   0  | 
add y 25    |      |            |  25  | 
mul y x     |      |            |      | 
add y 1     |      |            |      | 
mul z y     |      |            |      | 
mul y 0     |      |            |      | 
add y w     |      |            |      | 
add y p3    |      |            |      | 
mul y x     |      |            |      | 
add z y     |      |            |      | 


 */