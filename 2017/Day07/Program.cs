using System;
using System.Diagnostics;
using System.IO;

namespace ProgramTree
{
    class Program
    {
        static void Main(string[] args)
        {
            Run(() => Tree.Parse(File.ReadAllText("input.txt")).Root.Label);
            Run(() => Tree.Parse(File.ReadAllText("input.txt")).FindInvalidNode().RebalancingWeight);
        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }
    }
}
