using static AdventOfCode.Year2020.Day01.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day01
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");
        static int[] numbers = input.Select(int.Parse).ToArray();

        internal static Result Part1() => Run(() => numbers.Part1());
        internal static Result Part2() => Run(() => numbers.Part2());

    }
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

    public static async Task<IEnumerable<int>> LinesToNumbers(this string filename)
        => from line in await File.ReadAllLinesAsync(filename)
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

namespace AdventOfCode
{
    public class Tests
    {
        [Fact]
        async Task TestPart1()
        {
            var numbers = (await "sample.txt".LinesToNumbers()).ToList();
            var result = numbers.Part1();
            Assert.Equal(514579, result);
        }

        [Fact]
        public async Task AllNumbersAreUnique()
        {
            var numbers = (await "input.txt".LinesToNumbers()).ToList();
            Assert.Equal(numbers.Count, numbers.Distinct().Count());
        }

        [Fact]
        async Task TestPart2()
        {
            var numbers = (await "sample.txt".LinesToNumbers()).ToList();
            var result = numbers.Part2();
            Assert.Equal(241861950, result);
        }
    }
}
