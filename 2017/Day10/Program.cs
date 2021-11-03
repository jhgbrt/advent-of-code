using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;

namespace Knots
{
    class Program
    {
        static void Main(string[] args)
        {

            var input = "206,63,255,131,65,80,238,157,254,24,133,2,16,0,1,3";
            Run(() =>
            {
                var result = KnotsHash.Hash(input.Split(',').Select(byte.Parse).ToArray());
                var value = result[0] * result[1];
                return value;
            });

            Run(() => KnotsHash.Hash(input));

        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} in {sw.Elapsed}");
        }
    }
}
