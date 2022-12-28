namespace AdventOfCode.Year2020.Day01;

public class AoC202001
{
    static string[] input = Read.InputLines();
    static int[] numbers = input.Select(int.Parse).ToArray();

    public object Part1() => numbers.Part1();
    public object Part2() => numbers.Part2();


}
record Pair(int i, int j)
{
    public int Sum => i + j;
}
record Triplet(int i, int j, int k)
{
    public int Sum => i + j + k;
}


static class Driver
{
    public static long Part1(this IEnumerable<int> numbers)
        => (
            from p in numbers.GetPairs()
            where p.Sum == 2020
            select p.i
            ).Distinct().Aggregate(1L, (i, m) => m * i);

    public static long Part2(this IEnumerable<int> numbers)
        => (
            from p in numbers.GetTriplets()
            where p.Sum == 2020
            select p.i
            ).Distinct().Aggregate(1L, (i, m) => m * i);

    public static IEnumerable<Pair> GetPairs(this IEnumerable<int> numbers)
        => from i in numbers
           from j in numbers
           select new Pair(i, j);

    public static IEnumerable<Triplet> GetTriplets(this IEnumerable<int> numbers)
        => from i in numbers
           from j in numbers
           from k in numbers
           select new Triplet(i, j, k);
}