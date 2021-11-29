namespace AdventOfCode.Year2018.Day19;

class OpCode
{
    public static IDictionary<string, Action<long[], int, int, int>> All = new Action<long[], int, int, int>[]
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

    public static long[] apply(long[] input, (string name, int a, int b, int c) instruction)
    {
        var result = input.ToArray();
        var action = All[instruction.name];
        action(result, instruction.a, instruction.b, instruction.c);
        return result;
    }

    public static void addr(long[] r, int a, int b, int c) => r[c] = r[a] + r[b];
    public static void addi(long[] r, int a, int b, int c) => r[c] = r[a] + b;
    public static void mulr(long[] r, int a, int b, int c) => r[c] = r[a] * r[b];
    public static void muli(long[] r, int a, int b, int c) => r[c] = r[a] * b;
    public static void banr(long[] r, int a, int b, int c) => r[c] = r[a] & r[b];
    public static void bani(long[] r, int a, int b, int c) => r[c] = r[a] & b;
    public static void borr(long[] r, int a, int b, int c) => r[c] = r[a] | r[b];
    public static void bori(long[] r, int a, int b, int c) => r[c] = r[a] | b;
    public static void setr(long[] r, int a, int b, int c) => r[c] = r[a];
    public static void seti(long[] r, int a, int b, int c) => r[c] = a;
    public static void gtir(long[] r, int a, int b, int c) => r[c] = a > r[b] ? 1 : 0;
    public static void gtri(long[] r, int a, int b, int c) => r[c] = r[a] > b ? 1 : 0;
    public static void gtrr(long[] r, int a, int b, int c) => r[c] = r[a] > r[b] ? 1 : 0;
    public static void eqir(long[] r, int a, int b, int c) => r[c] = a == r[b] ? 1 : 0;
    public static void eqri(long[] r, int a, int b, int c) => r[c] = r[a] == b ? 1 : 0;
    public static void eqrr(long[] r, int a, int b, int c) => r[c] = r[a] == r[b] ? 1 : 0;

}