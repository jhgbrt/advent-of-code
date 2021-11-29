using static AdventOfCode.Year2015.Day17.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2015.Day17
{
    partial class AoC
    {
        static int[] input = new[] { 43, 3, 4, 10, 21, 44, 4, 6, 47, 41, 34, 17, 17, 44, 36, 31, 46, 9, 27, 38 };

        internal static Result Part1() => Run(() => Part1(input, 150));
        internal static Result Part2() => Run(() => Part2(input, 150));
        public static int Part1(int[] input, int sum) => Combinations(input).Where(c => c.Sum() == sum).Count();
        public static int Part2(int[] input, int sum)
        {
            var array = Combinations(input).OrderBy(c => c.Length).Where(c => c.Sum() == sum).ToArray();
            var minlength = array.First().Length;
            return array.Where(c => c.Length == minlength && c.Sum() == sum).Count();
        }
        static IEnumerable<T[]> Combinations<T>(T[] data) => Enumerable
          .Range(0, 1 << (data.Length))
          .Select(index => data.Where((v, i) => (index & (1 << i)) != 0).ToArray());
    }
}





