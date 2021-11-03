using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode
{
    static class Program
    {
        public static async Task Main()
        {
            var input = await File.ReadAllTextAsync("input.txt");

            Measure(() => AoC.Part1(input));

            Measure(() => AoC.Part2(input));
        }

        static void Measure<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"result = {result} - {sw.Elapsed}");
        }
    }
}
    