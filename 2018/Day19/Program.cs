using static AdventOfCode.Year2018.Day19.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day19
{
    partial class AoC
    {
        static string[] input = File.ReadAllLines("input.txt");

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));
        public static long Part1(string[] input)
        {
            var cpu = new CPU(int.Parse(input[0].Split(' ').Last()), input.GetInstructions(), new[] { 0L, 0, 0, 0, 0, 0 });
            cpu.Run();
            return cpu.Registers[0];
        }

        public static long Part2(string[] input)
        {
            var cpu = new CPU(int.Parse(input[0].Split(' ').Last()), input.GetInstructions(), new[] { 1L, 0, 0, 0, 0, 0 });
            return cpu.RunReverseEngineered().A;
        }
    }
}
