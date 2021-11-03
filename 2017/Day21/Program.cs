using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        var rules = File.ReadLines("input.txt").Select(Rule.Parse).ToArray();
        var input = ".#.\r\n..#\r\n###".ReadLines().ToRectangular();
        Run(() => new ExpandingGrid(input).Expand(rules, 5).Count('#'));
        Run(() => new ExpandingGrid(input).Expand(rules, 18).Count('#'));

    }



    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}
