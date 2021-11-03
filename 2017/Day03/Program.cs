using System;
using System.Diagnostics;
using System.Linq;
using static Spiral.Spiral;

namespace Spiral
{
    class Program
    {
        private const int Input = 265149;

        static void Main(string[] args)
        {
            Run(() => DistanceToOrigin(Input));
            Run(() => SpiralValues().SkipWhile(i => i.value <= Input).First().value);
        }

        static void Run<T>(Func<T> f)
        {
            var sw = Stopwatch.StartNew();
            var result = f();
            Console.WriteLine($"{result} - {sw.Elapsed}");
        }
    }
}
