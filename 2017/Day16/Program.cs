using System;
using System.Diagnostics;
using System.IO;

namespace dance
{
    class Program
    {
        static void Main(string[] args)
        {
            var dancer = new Dancer(new StreamReader("input.txt"));
            var initial = "abcdefghijklmnop";
            Run(() => dancer.Run(initial));
            Run(() => dancer.Run(initial, 1000000000));
        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }


    }
}
