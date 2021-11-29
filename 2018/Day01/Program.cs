using static AdventOfCode.Year2018.Day01.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day01
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));

        public static int Part1(string[] input) => input.Select(int.Parse).Sum();

        public static int Part2(string[] input)
        {
            var ints = input.Select(int.Parse);
            var frequency = 0;
            var hashSet = new HashSet<int>();
            ints.EndlessRepeat().TakeWhile(i =>
            {
                hashSet.Add(frequency);
                frequency += i;
                return !hashSet.Contains(frequency);
            }).Last();
            return frequency;
        }
    }
}
