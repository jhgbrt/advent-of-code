namespace AdventOfCode.Year2016.Day21;
using static Direction;

public class AoC201621
{
    static string[] input = Read.InputLines();
    static string plaintext = "abcdefgh";
    static string password = "fbgdceah";

    static IEnumerable<(int i, int j, int r)> query = from x in plaintext.Select((c, i) => (c, i))
                                                      let c = x.c
                                                      let i = x.i
                                                      let r = i < 4 ? i + 1 : i + 2
                                                      let t = new Rotate(r, Right).Apply(new StringBuilder(plaintext)).ToString()
                                                      let j = t.IndexOf(c)
                                                      select (i, j, r);
    static ImmutableArray<int> forward = (from x in query orderby x.i select x.r).ToImmutableArray();
    static ImmutableArray<int> reverse = (from x in query orderby x.j select x.r).ToImmutableArray();

    static IEnumerable<Instruction> Instructions => from line in input
                                                    let split = line.Split(' ')
                                                    let item = (split[0], split[1]) switch
                                                    {
                                                        ("move", "position") => new Move(int.Parse(split[2]), int.Parse(split[5])) as Instruction,
                                                        ("swap", "position") => new SwapP(int.Parse(split[2]), int.Parse(split[5])),
                                                        ("swap", "letter") => new SwapL(split[2][0], split[5][0]),
                                                        ("reverse", "positions") => new Reverse(int.Parse(split[2]), int.Parse(split[4])),
                                                        ("rotate", "left") => new Rotate(int.Parse(split[2]), Left),
                                                        ("rotate", "right") => new Rotate(int.Parse(split[2]), Right),
                                                        ("rotate", "based") => new Rotate2(forward, reverse, split[6][0], Right)
                                                    }
                                                    select item;

    public string Part1() => Instructions.Aggregate(new StringBuilder(plaintext), (sb, op) => op.Apply(sb)).ToString();
    public object Part2() => Instructions.Reverse().Aggregate(new StringBuilder(password), (sb, op) => op.Reverse().Apply(sb)).ToString();
}

interface Instruction
{
    StringBuilder Apply(StringBuilder input);
    Instruction Reverse();
}

readonly record struct Move(int from, int to) : Instruction
{
    public StringBuilder Apply(StringBuilder input)
    {
        var c = input[from];
        return input.Remove(from, 1).Insert(to, c);
    }
    public Instruction Reverse() => new Move(to, from);
}

readonly record struct SwapP(int x, int y) : Instruction
{
    public StringBuilder Apply(StringBuilder input)
    {
        (input[x], input[y]) = (input[y], input[x]);
        return input;
    }
        public Instruction Reverse() => this;
    }

readonly record struct SwapL(char a, char b) : Instruction
{
    public StringBuilder Apply(StringBuilder input) => new SwapP(input.IndexOf(a), input.IndexOf(b)).Apply(input);
    public Instruction Reverse() => this;
}

readonly record struct Reverse(int x, int y) : Instruction
{
    public StringBuilder Apply(StringBuilder input)
    {
        var sb = new StringBuilder();
        for (int i = y; i >= x; i--)
        {
            sb.Append(input[i]);
        }
        input.Remove(x, sb.Length);
        input.Insert(x, sb);
        return input;
    }
    Instruction Instruction.Reverse() => this;
}

enum Direction
{
    Left, Right
}
readonly record struct Rotate(int n, Direction d) : Instruction
{
    public StringBuilder Apply(StringBuilder input)
    {
        var d = this.d;
        return Range(0, n).Aggregate(input, (sb, _) => d switch
        {
            Left => sb.Append(sb[0]).Remove(0, 1),
            Right => sb.Insert(0, sb[sb.Length - 1]).Remove(sb.Length - 1, 1)
        });
    }
    public Instruction Reverse() => new Rotate(n, d switch
    {
        Left => Right,
        Right => Left
    });
}

readonly record struct Rotate2(ImmutableArray<int> forward, ImmutableArray<int> reverse, char l, Direction d) : Instruction
{
    public StringBuilder Apply(StringBuilder input)
    {
        int index = input.IndexOf(l);
        var r = d switch
        {
            Right => forward[index],
            Left => reverse[index]
        };
        return new Rotate(r, d).Apply(input);
    }
    public Instruction Reverse() => new Rotate2(forward, reverse, l, d switch
    {
        Right => Left,
        Left => Right
    });
}
