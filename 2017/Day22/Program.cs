using System;
using System.Diagnostics;
using System.IO;
using System.Linq;


class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadLines("input.txt").ToArray();

        Run(() =>
        {
            var grid = input.ToRectangular();
            return new Grid(grid).InfectGrid(10000);
        });

        Run(() =>
        {
            var grid = input.ToRectangular();
            return new Grid(grid).InfectGrid2(10000000);
        });
    }

    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}
