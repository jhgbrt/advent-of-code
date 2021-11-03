using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Passphrase
{
    static class Program
    {
        static void Main(string[] args)
        {
            Run(() => new StreamReader("input.txt").ReadLines().Count(IsValidPassword1));
            Run(() => new StreamReader("input.txt").ReadLines().Count(IsValidPassword2));
        }

        private static bool IsValidPassword1(string line)
        {
            var words = line.Split(' ');
            return words.Length == words.Distinct().Count();
        }
        private static bool IsValidPassword2(string line)
        {
            var words = line.Split(' ').Select(w => new string(w.OrderBy(c => c).ToArray())).ToArray();
            return words.Length == words.Distinct().Count();
        }

        static IEnumerable<string> ReadLines(this TextReader reader)
        {
            while (reader.Peek() >= 0) yield return reader.ReadLine();
        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }
    }
}
