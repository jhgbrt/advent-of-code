using static AdventOfCode.Year2016.Day01.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2016.Day01
{
    partial class AoC
    {
        static bool test = false;
        public static string input = File.ReadAllText(test ? "sample.txt" : "input.txt");

        internal static Result Part1() => Run(() => Navigate(input).Part1);
        internal static Result Part2() => Run(() => Navigate(input).Part2);

        internal static Navigator Navigate(string input)
        {
            var navigator = new Navigator();

            var instructions = input.Parse();
            foreach (var (direction, distance) in instructions)
            {
                navigator.Head(direction, distance);
            }

            return navigator;
        }

    }
}



public enum Bearing
{
    N = 0, E, S, W
}
public enum Direction
{
    L, R
}
