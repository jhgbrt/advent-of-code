using System;
using System.Diagnostics;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        var input = File.ReadAllText("input.txt");
        Run(() => Captcha.Calculate(input, 1));
        Run(() => Captcha.Calculate(input, 1));
        Run(() => Captcha.Calculate(input, input.Length/2));
    }

    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}