using System.Numerics;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");
    static int[] weights = (from line in input select int.Parse(line)).ToArray();

    internal static Result Part1() => Run(() => CalculateRecursive(weights, weights.Sum() / 3, 0, 1, 0));
    internal static Result Part2() => Run(() => CalculateRecursive(weights, weights.Sum() / 4, 0, 1, 0));

    static BigInteger CalculateRecursive(int[] weights, int target, int index, BigInteger entanglement, int totalweight)
    {
        if (totalweight == target) return entanglement;
        if (index >= weights.Length || totalweight > target) return -1;
        var l = CalculateRecursive(weights, target, index + 1, entanglement * weights[index], totalweight + weights[index]);
        var r = CalculateRecursive(weights, target, index + 1, entanglement, totalweight);
        if (l == -1) return r;
        if (r == -1) return l;
        return BigInteger.Min(l, r);
    }
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(new BigInteger(11846773891), Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(new BigInteger(80393059), Part2().Value);

}

