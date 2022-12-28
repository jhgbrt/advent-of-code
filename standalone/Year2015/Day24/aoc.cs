using System.Numerics;

var input = File.ReadAllLines("input.txt");
var weights = (
    from line in input
    select int.Parse(line)).ToArray();
var sw = Stopwatch.StartNew();
var part1 = CalculateRecursive(weights, weights.Sum() / 3, 0, 1, 0);
var part2 = CalculateRecursive(weights, weights.Sum() / 4, 0, 1, 0);
Console.WriteLine((part1, part2, sw.Elapsed));
BigInteger CalculateRecursive(int[] weights, int target, int index, BigInteger entanglement, int totalweight)
{
    if (totalweight == target)
        return entanglement;
    if (index >= weights.Length || totalweight > target)
        return -1;
    var l = CalculateRecursive(weights, target, index + 1, entanglement * weights[index], totalweight + weights[index]);
    var r = CalculateRecursive(weights, target, index + 1, entanglement, totalweight);
    if (l == -1)
        return r;
    if (r == -1)
        return l;
    return BigInteger.Min(l, r);
}