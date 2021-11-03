using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

class Program
{
    static void Main(string[] args)
    {
        var maze = File.ReadAllLines("input.txt");
        Run(() => new MazeRunner(maze).Traverse());
    }
    
    static void Run<T>(Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"{result} - {sw.Elapsed}");
    }
}
