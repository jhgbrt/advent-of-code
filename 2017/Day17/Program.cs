using System;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        const int input = 377;
        Run(() =>
        {
            var result = Spinlock.Find(input, 2017);
            return result.buffer[result.index + 1];
        });
        Run(() => Spinlock.FindFast(input, 2017));
        Run(() => Spinlock.FindFast(input, 50_000_000));
    }

    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}
