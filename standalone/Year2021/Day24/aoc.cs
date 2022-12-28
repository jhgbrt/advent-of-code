var input = File.ReadAllLines("input.txt");
var program = input.Select(Parse).ToImmutableArray();
var foldedInput = FoldInput();
var regex = new Regex(@"inp w     mul x 0   add x z   mod x 26  div z (?<p1>\d+) +add x (?<p2>[-\d]+) +eql x w   eql x 0   mul y 0   add y 25  mul y x   add y 1   mul z y   mul y 0   add y w   add y (?<p3>\d+) +mul y x   add z y");
var parameters = (
    from line in foldedInput
    let match = regex.Match(line)
    select (p1: match.GetInt32("p1"), p2: match.GetInt32("p2"), p3: match.GetInt32("p3"))).ToImmutableArray();
var sw = Stopwatch.StartNew();
var part1 = FindValidNumber(1);
var part2 = FindValidNumber(2);
Console.WriteLine((part1, part2, sw.Elapsed));
List<string> FoldInput()
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

Instruction Parse(string line) => new Instruction(GetFunction(line), line);
Func<Memory, IEnumerator<int>, Memory> GetFunction(string line)
{
    var split = line.Split();
    var register1 = split[1][0];
    if (split.Length == 2)
    {
        return (memory, input) =>
        {
            if (!input.MoveNext())
                throw new InvalidOperationException("no more inputs!");
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

Memory Evaluate(Memory memory, ImmutableArray<Instruction> program, IEnumerator<int> input)
{
    foreach (var instruction in program)
    {
        memory = instruction.F(memory, input);
        Console.WriteLine($"--{memory}");
    }

    return memory;
}

[Fact]
void TestProgram1()
{
    var program = new[] { "inp x", "mul x -1" }.Select(Parse).ToImmutableArray();
    var result = Evaluate(new Memory(), program, Repeat(5, 1).GetEnumerator());
    Assert.Equal(-5, result.x);
}

[Fact]
void TestProgram2()
{
    var program = new[] { "inp w", "add z w", "mod z 2", "div w 2", "add y w", "mod y 2", "div w 2", "add x w", "mod x 2", "div w 2", "mod w 2" }.Select(Parse).ToImmutableArray();
    var result = Evaluate(new Memory(), program, Repeat(0b1101, 1).GetEnumerator());
    Assert.Equal(new Memory(1, 1, 0, 1), result);
}

IEnumerable<int> CalculatePreviousZ(int w, int p1, int p2, int p3, int z)
{
    var x = z - w - p3;
    if (x % 26 == 0)
    {
        yield return x / 26 * p1;
    }

    if ((0..26).Contains(w - p2))
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
    var zvalues = new HashSet<int>()
    {0};
    // cache the digits which lead to a specific z-value (list will contain 14 values at the end)
    var results = new Dictionary<int, ImmutableList<int>>();
    var digits = Range(1, 9).ToArray();
    if (part == 2)
        digits = digits.Reverse().ToArray();
    foreach (var (p1, p2, p3) in parameters.Reverse())
    {
        var q =
            from z in zvalues
            from w in digits
            from previous in CalculatePreviousZ(w, p1, p2, p3, z)
            let list = results.ContainsKey(z) ? results[z] : ImmutableList<int>.Empty
            select (previous, list.Insert(0, w));
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

static class Ex
{
    public static bool Contains(this Range r, int v) => r.Start.Value <= v && v < r.End.Value;
}