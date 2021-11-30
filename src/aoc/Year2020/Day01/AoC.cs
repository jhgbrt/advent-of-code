namespace AdventOfCode.Year2020.Day01;

public class AoC202001 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC202001));
    static int[] numbers = input.Select(int.Parse).ToArray();

    public override object Part1() => numbers.Part1();
    public override object Part2() => numbers.Part2();

}
record Pair(int i, int j)
{
    public int Sum => i + j;
}
record Triplet(int i, int j, int k)
{
    public int Sum => i + j + k;
}
