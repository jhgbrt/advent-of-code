using System;
using System.Diagnostics;
using System.IO;

namespace AdventOfCode
{
    static class Program
    {
        public static void Main()
        {
            using (var stream = File.OpenRead("input.txt"))
            using (var reader = new StreamReader(stream))
            {
                Measure(() => AoC.Part1(reader));
            }

            using (var stream = File.OpenRead("input.txt"))
            using (var reader = new StreamReader(stream))
            {
                Measure(() => AoC.Part2(reader));
            }
        }

        static void Measure<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"result = {result} - {sw.Elapsed}");
        }
        
    }
}
