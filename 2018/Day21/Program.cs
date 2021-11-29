
using static AdventOfCode.Year2018.Day21.AoC;


Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day21
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");
        internal static Result Part1() => Run(() =>
        {
            var cpu = new CPU(int.Parse(input[0].Split(' ').Last()), input.GetInstructions(), new long[] { 0, 0, 0, 0, 0, 0 });
            Console.WriteLine(cpu.Run(false));
            return cpu.RunReverseEngineered(false);
        });
        internal static Result Part2() => Run(() =>
        {
            var cpu = new CPU(int.Parse(input[0].Split(' ').Last()), input.GetInstructions(), new long[] { 0, 0, 0, 0, 0, 0 });
            return cpu.RunReverseEngineered(true);
        });
        internal static void WriteProgram()
        {
            foreach (var i in input.GetInstructions()) Console.WriteLine(i);
        }
    }
}

static class Ex
{
    public static IEnumerable<Instruction> GetInstructions(this string[] input)
    {
        for (int i = 1; i < input.Length; i++)
        {
            var instruction = input[i].Split(' ');
            yield return new(i - 1, instruction[0], int.Parse(instruction[1]), int.Parse(instruction[2]), int.Parse(instruction[3]));
        }
    }
}

class CPU
{
    private long _ip;
    private readonly long _ipregister;
    private readonly ImmutableArray<Instruction> _instructions;
    private long[] _registers;
    public long[] Registers => _registers;
    public CPU(long ip, IEnumerable<Instruction> instructions, long[] registers)
    {
        _ip = 0;
        _ipregister = ip;
        _instructions = instructions.ToImmutableArray();
        _registers = registers;
    }

#pragma warning disable CS0164
#pragma warning disable IDE0054
#pragma warning disable IDE0059
#pragma warning disable IDE1006

    public long RunReverseEngineered(bool part2)
    {
        var set = new HashSet<long>();
        (long A, long B, long C, long D, long E, long I) = (0, 0, 0, 0, 0, 0);
        long last = -1;
    _00: I = 00; E = 123;
    _01: I = 01; E = E & 456;
    _02: I = 02; E = E == 72 ? 1 : 0;
    _03: I = 03; I = E + I; if (E == 1) { goto _05; } else if (E != 0) throw new InvalidOperationException();
        _04: I = 04; I = 0; goto _01;
    _05: I = 05; E = 0;
    _06: I = 06; D = E | 65536;
    _07: I = 07; E = 733884;
    _08: I = 08; B = D & 255;
    _09: I = 09; E = E + B;
    _10: I = 10; E = E & 16777215;
    _11: I = 11; E = E * 65899;
    _12: I = 12; E = E & 16777215;
    _13: I = 13; B = 256 > D ? 1 : 0;
    _14: I = 14; I = B + I; if (B == 1) { goto _16; } else if (B != 0) throw new InvalidOperationException();
        _15: I = 15; I = I + 1; goto _17;
    _16: I = 16; I = 27; goto _28;
    _17: I = 17; B = 0;
    _18: I = 18; C = B + 1;
    _19: I = 19; C = C * 256;
    _20: I = 20; C = C > D ? 1 : 0;
    _21: I = 21; I = C + I; if (C == 1) { goto _23; } else if (C != 0) throw new InvalidOperationException();
        _22: I = 22; I = I + 1; goto _24;
    _23: I = 23; I = 25; goto _26;
    _24: I = 24; B = B + 1;
    _25: I = 25; I = 17; goto _18;
    _26: I = 26; D = B;
    _27: I = 27; I = 7; goto _08;
    _28: I = 28; B = E == A ? 1 : 0;
        if (last == -1 && !part2)
        {
            return E;
        }
        if (set.Contains(E))
        {
            return last;
        }
        last = E;
        set.Add(last);
    _29: I = 29; I = B + I; if (B == 1) { goto _31; } else if (B != 0) throw new InvalidOperationException();
        _30: I = 30; I = 5; goto _06;
    _31:;

        return E;
    }
    public long Run(bool part2)
    {

        HashSet<long> seen = new();
        long last = -1;
        while (_ip >= 0 && _ip < _instructions.Length)
        {
            if (_ip == 28)
            {
                if (!part2 && last == -1)
                    return _registers[5];
                if (seen.Contains(_registers[5]))
                    return last;
                last = _registers[5];
                seen.Add(last);
            }

            _registers[_ipregister] = _ip;

            Instruction instruction = _instructions.ElementAt((int)_ip);
            _registers = OpCode.apply(_registers, instruction);
            _ip = _registers[_ipregister] + 1;
        }

        return -1;
    }
}
class OpCode
{
    public static IReadOnlyDictionary<string, Action<long[], long, long, long>> All = new[]
    {
            addr,
            addi,
            mulr,
            muli,
            banr,
            bani,
            borr,
            bori,
            setr,
            seti,
            gtir,
            gtri,
            gtrr,
            eqir,
            eqri,
            eqrr,
    }.ToDictionary(x => x.Method.Name);

    public static long[] apply(IEnumerable<long> input, Instruction instruction)
    {
        var result = input.ToArray();
        var action = All[instruction.name];
        action(result, instruction.a, instruction.b, instruction.c);
        return result;
    }

    public static void addr(long[] r, long a, long b, long c) => r[c] = r[a] + r[b];
    public static void addi(long[] r, long a, long b, long c) => r[c] = r[a] + b;
    public static void mulr(long[] r, long a, long b, long c) => r[c] = r[a] * r[b];
    public static void muli(long[] r, long a, long b, long c) => r[c] = r[a] * b;
    public static void banr(long[] r, long a, long b, long c) => r[c] = r[a] & r[b];
    public static void bani(long[] r, long a, long b, long c) => r[c] = r[a] & b;
    public static void borr(long[] r, long a, long b, long c) => r[c] = r[a] | r[b];
    public static void bori(long[] r, long a, long b, long c) => r[c] = r[a] | b;
    public static void setr(long[] r, long a, long b, long c) => r[c] = r[a];
    public static void seti(long[] r, long a, long b, long c) => r[c] = a;
    public static void gtir(long[] r, long a, long b, long c) => r[c] = a > r[b] ? 1 : 0;
    public static void gtri(long[] r, long a, long b, long c) => r[c] = r[a] > b ? 1 : 0;
    public static void gtrr(long[] r, long a, long b, long c) => r[c] = r[a] > r[b] ? 1 : 0;
    public static void eqir(long[] r, long a, long b, long c) => r[c] = a == r[b] ? 1 : 0;
    public static void eqri(long[] r, long a, long b, long c) => r[c] = r[a] == b ? 1 : 0;
    public static void eqrr(long[] r, long a, long b, long c) => r[c] = r[a] == r[b] ? 1 : 0;

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
        if (c != 4) return string.Empty;

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