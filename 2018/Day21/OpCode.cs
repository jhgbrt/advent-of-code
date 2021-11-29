namespace AdventOfCode.Year2018.Day21;

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