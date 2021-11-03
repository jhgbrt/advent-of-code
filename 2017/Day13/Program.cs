using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace firewall
{
    class Program
    {
        static void Main(string[] args)
        {
            var items = (
                from line in File.ReadLines("input.txt")
                let indexes = line.Split(": ").Select(int.Parse).ToArray()
                select (layer: indexes[0], range: indexes[1])
                ).ToArray();

            Run(() => Firewall.Severity(items));

            Run(() => Firewall.DelayToEscape(items));

        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} / {sw.Elapsed}");
        }
    }

    static class Ex
    {
        public static IEnumerable<string> ReadLines(this TextReader input)
        {
            while (input.Peek() >= 0) yield return input.ReadLine();
        }
    }
}
