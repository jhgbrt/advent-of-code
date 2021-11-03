using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Plumber
{
    static class Program
    {
        static void Main(string[] args)
        {
            var edges = (
                from line in File.ReadLines("input.txt")
                let parts = line.Split("<->").Select(s => s.Trim()).ToArray()
                let vertex1 = int.Parse(parts[0])
                from vertex2 in parts[1].Split(',').Select(int.Parse)
                select (vertex1: vertex1, vertex2: vertex2)
            );


            Run(() => new Graph(edges).Count(0));
            Run(() => new Graph(edges).SubGraphs().Count);
        }
        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }

    }

}
