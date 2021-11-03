using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllText("input.txt");
        Run(() => CheckSum.CheckSum1(new StringReader(input)));
        Run(() => CheckSum.CheckSum2(new StringReader(input)));
    }

    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }

}
