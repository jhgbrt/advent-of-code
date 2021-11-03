using System;
using System.Collections.Generic;
using System.Linq;
using static System.Math;

namespace hexgrid
{
    static class HexGrid
    {
        public static (int distance, int max) Calculate(string[] steps)
        {
            var aggregate = steps.Aggregate(
                (coordinate: (x:0, y:0, z:0), max: int.MinValue),
                (tuple, s) => (
                    Step(tuple.coordinate, s),
                    Max(tuple.max, tuple.coordinate.Items().Max(Abs))
                )
            );
            var (coordinate, max) = aggregate;
            return (coordinate.Items().Max(Abs), max);
        }
        private static (int x, int y, int z) Step((int x, int y, int z) coordinate, string step)
        {
            var (x, y, z) = coordinate;
            switch (step)
            {
                case "n": return (x, y + 1, z - 1);
                case "nw": return (x - 1, y + 1, z);
                case "ne": return (x + 1, y, z - 1);
                case "s": return (x, y - 1, z + 1);
                case "sw": return (x - 1, y, z + 1);
                case "se": return (x + 1, y - 1, z);
                default:
                    throw new InvalidOperationException();
            }
        }

        static IEnumerable<T> Items<T>(this (T, T, T) tuple)
        {
            yield return tuple.Item1;
            yield return tuple.Item2;
            yield return tuple.Item3;
        }
    }
}