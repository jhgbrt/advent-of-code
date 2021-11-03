using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace AdventOfCode
{
    static class Program
    {
        public static void Main()
        {
            //var input = await File.ReadAllTextAsync("input.txt");

            Measure(() => AoC.Part1(465, 71498));

            Measure(() => AoC.Part2(465, 71498));
        }

        static void Measure<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"result = {result} - {sw.Elapsed}");
        }
        
    }
}
