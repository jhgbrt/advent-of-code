namespace AdventOfCode.Year2021.Day24;

public class AoC202124
{
    static string[] input = Read.InputLines();

    static ImmutableArray<Instruction> program = input.Select(Parse).ToImmutableArray();

    static List<string> foldedInput = FoldInput();

    public object Part1() => FindValidNumber(1);
    public object Part2() => FindValidNumber(2);

    static List<string> FoldInput()
    {
        List<string> items = new();
        var sb = new StringBuilder();
        foreach (var instruction in input)
        {
            if (sb.Length > 0 && instruction.StartsWith("inp"))
            {
                items.Add(sb.ToString());
                sb.Clear();
            }
            sb.Append(instruction.PadRight(10));
        }
        items.Add(sb.ToString());
        return items;
    }

    static Instruction Parse(string line) => new Instruction(GetFunction(line), line);
    static Func<Memory, IEnumerator<int>, Memory> GetFunction(string line)
    {
        var split = line.Split();
        var register1 = split[1][0];

        if (split.Length == 2)
        {
            return (memory, input) =>
            {
                if (!input.MoveNext()) throw new InvalidOperationException("no more inputs!");
                var value = input.Current;
                return memory.SetValue(register1, value);
            };
        }
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
    private static Memory Evaluate(Memory memory, ImmutableArray<Instruction> program, IEnumerator<int> input)
    {
        foreach (var instruction in program)
        {
            memory = instruction.F(memory, input);
            Console.WriteLine($"--{memory}");
        }
        return memory;
    }

    // Test the 'naive' implementation
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

    // reverse engineered solution
    // Notice that program contains similar instructions.
    /*
    foreach (var line in items)
        Console.WriteLine(line);

    This gives:

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

    // equivalent to:
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

     */

    static Regex regex = new Regex(@"inp w     mul x 0   add x z   mod x 26  div z (?<p1>\d+) +add x (?<p2>[-\d]+) +eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y (?<p3>\d+) +mul y x   add z y");

    static ImmutableArray<(int p1, int p2, int p3)> parameters = (
            from line in foldedInput
            let match = regex.Match(line)
            select (p1: match.GetInt32("p1"), p2: match.GetInt32("p2"), p3: match.GetInt32("p3"))
            ).ToImmutableArray();

    IEnumerable<int> CalculatePreviousZ(int w, int p1, int p2, int p3, int z)
    {
        var x = z - w - p3;
        if (x % 26 == 0)
        {
            yield return x / 26 * p1;
        }
        if ((0..26).Contains(w-p2))
        {
            yield return w - p2 + z * p1;
        }
    }

    long FindValidNumber(int part)
    {
        // find possible input values for which result (z) = 0
        // keep a list of digits that, for each parameter combination (in reverse)
        // leads to the final result (0)

        // the set of z-values for which to calculate the previous z-value(s) leading to this number
        var zvalues = new HashSet<int>() { 0 };

        // cache the digits which lead to a specific z-value (list will contain 14 values at the end)
        var results = new Dictionary<int, ImmutableList<int>>();
        var digits = Range(1, 9).ToArray();
        if (part == 2) digits = digits.Reverse().ToArray();
        foreach (var (p1,p2,p3) in parameters.Reverse())
        {
            var q = from z in zvalues
                    from w in digits
                    from previous in CalculatePreviousZ(w, p1, p2, p3, z)
                    let list = results.ContainsKey(z) ? results[z] : ImmutableList<int>.Empty
                    select (previous, list.Insert(0,w));

            var previousvalues = new HashSet<int>();
            foreach (var (previous, list) in q)
            {
                previousvalues.Add(previous);
                results[previous] = list;
            }
            zvalues = previousvalues;
        }

        return results[0].Reverse().Select((d, i) => d * (long)Math.Pow(10, i)).Sum();
    }
}

static class SerialNumber
{
    public const long Min = 11111111111111;
    public const long Max = 99999999999999;
    public static IEnumerable<int> Digits(this long value)
    {
        var factor = 10000000000000;
        while (factor >= 1)
        {
            yield return (int)(value / factor % 10);
            factor /= 10;
        }
    }
}

readonly record struct Memory(int w, int x, int y, int z)
{
    public int GetValue(char register) => register switch
    {
        'w' => w,
        'x' => x,
        'y' => y,
        'z' => z,
        _ => throw new NotImplementedException()
    };

    public Memory SetValue(char register, int value) => register switch
    {
        'w' => this with { w = value },
        'x' => this with { x = value },
        'y' => this with { y = value },
        'z' => this with { z = value },
        _ => throw new NotImplementedException()
    };

}

record Instruction(Func<Memory, IEnumerator<int>, Memory> F, string Name);

static class Ex
{
    public static bool Contains(this Range r, int v) => r.Start.Value <= v && v < r.End.Value;
}