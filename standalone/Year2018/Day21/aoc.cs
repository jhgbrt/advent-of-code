var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = Part2();
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    var cpu = new CPU(int.Parse(input[0].Split(' ').Last()), input.GetInstructions(), new long[] { 0, 0, 0, 0, 0, 0 });
    return cpu.RunReverseEngineered(false);
}

object Part2()
{
    var cpu = new CPU(int.Parse(input[0].Split(' ').Last()), input.GetInstructions(), new long[] { 0, 0, 0, 0, 0, 0 });
    return cpu.RunReverseEngineered(true);
}

void WriteProgram()
{
    foreach (var i in input.GetInstructions())
        Console.WriteLine(i);
}

internal record struct Instruction(long address, string name, long a, long b, long c)
{
    public static implicit operator (long address, string name, long a, long b, long c)(Instruction value)
    {
        return (value.address, value.name, value.a, value.b, value.c);
    }

    string R(long n) => n switch
    {
        0 => "A",
        1 => "B",
        2 => "C",
        3 => "D",
        4 => "I",
        5 => "E",
        _ => throw new NotImplementedException()
    };
    public override string ToString()
    {
        return name switch
        {
            "addr" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} + {R(b)}         ".PadRight(50) + $"; {PrintJump()}",
            "addi" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} + {b}            ".PadRight(50) + $"; {PrintJump()}",
            "mulr" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} * {R(b)}         ".PadRight(50) + $"; {PrintJump()}",
            "muli" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} * {b}            ".PadRight(50) + $"; {PrintJump()}",
            "banr" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} & {R(b)}         ".PadRight(50) + $"; {PrintJump()}",
            "bani" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} & {b}            ".PadRight(50) + $"; {PrintJump()}",
            "borr" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} | {R(b)}         ".PadRight(50) + $"; {PrintJump()}",
            "bori" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} | {b}            ".PadRight(50) + $"; {PrintJump()}",
            "setr" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)}                  ".PadRight(50) + $"; {PrintJump()}",
            "seti" => $"_{address:00} : I = {address:00}; {R(c),3} = {a}                     ".PadRight(50) + $"; {PrintJump()}",
            "gtir" => $"_{address:00} : I = {address:00}; {R(c),3} = {a} > {R(b)} ? 1 : 0    ".PadRight(50) + $"; {PrintJump()}",
            "gtri" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} > {b} ? 1 : 0    ".PadRight(50) + $"; {PrintJump()}",
            "gtrr" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} > {R(b)} ? 1 : 0 ".PadRight(50) + $"; {PrintJump()}",
            "eqir" => $"_{address:00} : I = {address:00}; {R(c),3} = {a} == {R(b)} ? 1 : 0   ".PadRight(50) + $"; {PrintJump()}",
            "eqri" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} == {b} ? 1 : 0   ".PadRight(50) + $"; {PrintJump()}",
            "eqrr" => $"_{address:00} : I = {address:00}; {R(c),3} = {R(a)} == {R(b)} ? 1 : 0".PadRight(50) + $"; {PrintJump()}",
            _ => throw new NotImplementedException()
        };
    }

    string PrintJump()
    {
        if (c != 4)
            return string.Empty;
        return name switch
        {
            "seti" => $"goto _{a + 1:00};",
            "addi" => a switch
            {
                4 => $"goto _{address + b + 1:00};",
                _ => $"#error CAN NOT GOTO {R(a)} + {b}"
            },
            "addr" => (a, b) switch
            {
                (4, not 4) => $"if ({R(b)} == 1) {{ goto _{address + 2:00}; }} else if ({R(b)} != 0) throw new InvalidOperationException();",
                (not 4, 4) => $"if ({R(a)} == 1) {{ goto _{address + 2:00}; }} else if ({R(a)} != 0) throw new InvalidOperationException();",
                (4, 4) p => $"goto _{address + address + 1:00};",
                _ => string.Empty
            },
            _ => string.Empty
        };
    }
}