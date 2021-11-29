using static AdventOfCode.Year2019.Day01.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2019.Day01
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));
        public static int Part1(string[] input) => input.Select(int.Parse).Select(CalculateFuel1).Sum();

        public static int Part2(string[] input) => input.Select(int.Parse).Select(CalculateFuel2).Sum();
        public static int CalculateFuel1(int mass) => mass / 3 - 2;
        public static int CalculateFuel2(int mass) => Fuel(mass).Sum();

        static IEnumerable<int> Fuel(int mass)
        {
            var fuel = mass;
            while (true)
            {
                fuel = CalculateFuel1(fuel);
                if (fuel < 0) break;
                yield return fuel;
            }
        }
    }
}