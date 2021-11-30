namespace AdventOfCode.Year2020.Day01;

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

    public static IEnumerable<int> LinesToNumbers(this string filename)
        => from line in Read.Lines(typeof(AoCImpl), filename)
           select int.Parse(line);

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