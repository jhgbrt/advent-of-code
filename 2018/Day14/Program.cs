using System;
using System.Diagnostics;

namespace AdventOfCode
{
    static class Program
    {
        public static void Main()
        {
            Measure(() => AoC.Part1(327901));

            Measure(() => AoC.Part2(327901));
        }

        static void Measure<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"result = {result} - {sw.Elapsed}");
        }
        
    }
}
