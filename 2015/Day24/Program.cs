using System.Numerics;

using static AoC;

var lines = ReadInput(false);

var weights = (from line in lines select int.Parse(line)).ToArray();

Part1(() => CalculateRecursive(weights, weights.Sum() / 3, 0, 1, 0));
Part2(() => CalculateRecursive(weights, weights.Sum() / 4, 0, 1, 0));


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
