using System.Numerics;

using Xunit;
using Xunit.Abstractions;

public class Tests
{
    private readonly ITestOutputHelper output;
    private readonly int[] weights;
    public Tests(ITestOutputHelper output)
    {
        var filename = "input.txt";
        weights = (from line in File.ReadAllLines(filename) select int.Parse(line)).OrderByDescending(i => i).ToArray();
        //weights = new[] { 1, 2, 3, 4, 5, 7, 8, 9, 10, 11 };
        this.output = output;
    }

    [Fact]
    public void Test1()
    {
        //var combinations = from c1 in Combinations(weights)
        //                   where c1.Any()
        //                   let e = c1.Aggregate(1, (x,y) => x*y)
        //                   from c2 in Combinations(weights.Except(c1))
        //                   where c2.Any() && c1.Sum() == c2.Sum()
        //                   let c3 = weights.Except(c1.Concat(c2))
        //                   where c3.Any()
        //                   orderby c3.Count(), e
        //                   select (c1, c2, c3, e);

        //foreach ((var c1, var c2, var c3, var e) in combinations)
        //{
        //    output.WriteLine($"c1:{string.Join(',', c1)} - c2:{string.Join(',', c2)} - c3:{string.Join(',', c3)}, e = {e}");
        //}
        var sum = weights.Sum();
        output.WriteLine(MinProduct(weights, sum / 3, 0, 1, 0).ToString());

    }
    static BigInteger MinProduct(int[] weights, int weightrequirement, int index, BigInteger cumulativeproduct, int cumulativeweight)
    {
        if (cumulativeweight == weightrequirement)
            return cumulativeproduct;
        if (index >= weights.Length || cumulativeweight > weightrequirement)
        {
            return -1;
        }
        var lhs = MinProduct(weights, weightrequirement, index + 1, cumulativeproduct * weights[index], cumulativeweight + weights[index]);
        var rhs = MinProduct(weights, weightrequirement, index + 1, cumulativeproduct, cumulativeweight);
        if (lhs == -1) return rhs;
        if (rhs == -1) return lhs;
        return BigInteger.Min(lhs, rhs);
    }

    IEnumerable<int[]> Combinations(IEnumerable<int> input)
    {
        var data = input.ToArray();
        return from index in Enumerable.Range(0, 1 << (data.Length))
               select data.Where((v, i) => (index & (1 << i)) != 0).ToArray();
    }
}