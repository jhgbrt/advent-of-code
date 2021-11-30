namespace AdventOfCode.Year2018.Day21;

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