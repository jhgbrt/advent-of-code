namespace AdventOfCode.Year2018.Day19;

class CPU
{
    private long _ip;
    private readonly int _ipregister;
    private readonly (string code, int a, int b, int c)[] _instructions;
    private long[] _registers;
    public long[] Registers => _registers;
    public CPU(int ip, IEnumerable<(string code, int a, int b, int c)> instructions, long[] registers)
    {
        _ip = 0;
        _ipregister = ip;
        _instructions = instructions.ToArray();
        _registers = registers;
    }

    private IEnumerable<int> GetFactors(int x)
    {
        for (int f = 1; f * f <= x; f++)
        {
            if (x % f == 0)
            {
                yield return f;
                if (f * f != x)
                    yield return x / f;
            }
        }
    }

    public (int A, int I, int B, int C, int D, int E) RunReverseEngineered()
    {
        (int A, int I, int B, int C, int D, int E) = (1, 0, 0, 0, 0, 0);

        I = 17;
        B = (I + 2) * (B + 2) * (B + 2) * 11;
        D = (D + 7) * (I + 5) + 13;
        B = B + D;
        I = 26 + A;
        D = (I + 5) * 14 * (I + 3) * (I * (I + 1) + (I + 2));
        B = B + D;
        A = 0;
        I = I + 8;
        I = 1;

        //C = 1;                                  // 1
        //do
        //{
        //    E = 1;                                  // 2
        //    do
        //    {
        //        if (C * E == B)
        //        {
        //            A += C;                              // 7
        //        }
        //        E++;                              // 8
        //    } while (E <= B);
        //    C++;                              // 12
        //} while (C <= B);

        var factors = GetFactors(B).ToList();

        A = (
            from f1 in factors
            from f2 in factors
            where f1 * f2 == B
            select f1
            ).Sum();


        return (A, I, B, C, D, E);

        /*
                    I = I + 16;                             // 0
                    C = 1;                                  // 1
                    E = 1;                                  // 2
                    D = C * E;                              // 3
                    D = D == B ? 1 : 0;                     // 4
                    I = D + I;                              // 5
                    I = I + 1;                              // 6
                    A = C + A;                              // 7
                    E = E + 1;                              // 8
                    D = E > B ? 1 : 0;                      // 9
                    I = I + D;                              // 10
                    I = 2;                                  // 11
                    C = C + 1;                              // 12
                    D = C > B ? 1 : 0;                      // 13
                    I = D + I;                              // 14
                    I = 1;                                  // 15
                    I = I * I;                              // 16
                    B = B + 2;                              // 17
                    B = B * B;                              // 18
                    B = I * B;                              // 19
                    B = B * 11;                             // 20
                    D = D + 7;                              // 21
                    D = D * I;                              // 22
                    D = D + 13;                             // 23
                    B = B + D;                              // 24
                    I = I + A;                              // 25
                    I = 0;                                  // 26
                    D = I;                                  // 27
                    D = D * I;                              // 28
                    D = I + D;                              // 29
                    D = I * D;                              // 30
                    D = D * 14;                             // 31
                    D = D * I;                              // 32
                    B = B + D;                              // 33
                    A = 0;                                  // 34
                    I = 0;                                  // 35
        */
    }

    public void Run()
    {
        int i = 0;
        while (_ip >= 0 && _ip < _instructions.Length)
        {
            i++;
            var instruction = _instructions[_ip];
            _registers[_ipregister] = _ip;
            var registers = OpCode.apply(_registers, instruction);
            var ip = registers[_ipregister] + 1;
            _registers = registers;
            _ip = ip;
        }
    }
}