namespace AdventOfCode.Year2018.Day16;

class OpCode
{
    public static IList<Action<int[], int, int, int, int>> All = new Action<int[], int, int, int, int>[]
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
    };

    public static int[] apply(int[] input, (int code, int a, int b, int c) instruction, Action<int[], int, int, int, int> opcode)
    {
        int[] result = input.ToArray();
        opcode(result, instruction.code, instruction.a, instruction.b, instruction.c);
        return result;
    }

    public static void addr(int[] r, int code, int a, int b, int c) => r[c] = r[a] + r[b];
    public static void addi(int[] r, int code, int a, int b, int c) => r[c] = r[a] + b;
    public static void mulr(int[] r, int code, int a, int b, int c) => r[c] = r[a] * r[b];
    public static void muli(int[] r, int code, int a, int b, int c) => r[c] = r[a] * b;
    public static void banr(int[] r, int code, int a, int b, int c) => r[c] = r[a] & r[b];
    public static void bani(int[] r, int code, int a, int b, int c) => r[c] = r[a] & b;
    public static void borr(int[] r, int code, int a, int b, int c) => r[c] = r[a] | r[b];
    public static void bori(int[] r, int code, int a, int b, int c) => r[c] = r[a] | b;
    public static void setr(int[] r, int code, int a, int b, int c) => r[c] = r[a];
    public static void seti(int[] r, int code, int a, int b, int c) => r[c] = a;
    public static void gtir(int[] r, int code, int a, int b, int c) => r[c] = a > r[b] ? 1 : 0;
    public static void gtri(int[] r, int code, int a, int b, int c) => r[c] = r[a] > b ? 1 : 0;
    public static void gtrr(int[] r, int code, int a, int b, int c) => r[c] = r[a] > r[b] ? 1 : 0;
    public static void eqir(int[] r, int code, int a, int b, int c) => r[c] = a == r[b] ? 1 : 0;
    public static void eqri(int[] r, int code, int a, int b, int c) => r[c] = r[a] == b ? 1 : 0;
    public static void eqrr(int[] r, int code, int a, int b, int c) => r[c] = r[a] == r[b] ? 1 : 0;

}