using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        var components = (
            from line in File.ReadLines("input.txt")
            select Component.Parse(line)
        ).ToImmutableList();

        Run(() => Bridge.Longest(components));
        Run(() => Bridge.Strongest(components));
    }

    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}
