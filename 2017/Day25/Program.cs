using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Run(() => File.ReadAllText("input.txt").EncodeToSomethingSimpler().CalculateChecksum());
    }
    
    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}
