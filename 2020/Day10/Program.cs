using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

var example1 = new[] { 16,10,15,5,1,11,7,19,6,12,4 };
var example2 = new[] { 28, 33, 18, 42, 31, 14, 46, 20, 48, 47, 24, 23, 49, 45, 19, 38, 39, 11, 1, 32, 25, 35, 8, 17, 7, 9, 4, 2, 34, 10, 3 };
var input = new[] { 97, 62, 23, 32, 51, 19, 98, 26, 90, 134, 73, 151, 116, 76, 6, 94, 113, 127, 119, 44, 115, 50, 143, 150, 86, 91, 36, 104, 131, 101, 38, 66, 46, 96, 54, 70, 8, 30, 1, 108, 69, 139, 24, 29, 77, 124, 107, 14, 137, 16, 140, 80, 68, 25, 31, 59, 45, 126, 148, 67, 13, 125, 53, 57, 41, 47, 35, 145, 120, 12, 37, 5, 110, 138, 130, 2, 63, 83, 22, 79, 52, 7, 95, 58, 149, 123, 89, 109, 15, 144, 114, 9, 78 };

var array = input;

var ordered = (
    from i in new[] { 0 }.Concat<int>(array).Concat<int>(new[] { array.Max() + 3 }) orderby i select i
    ).ToArray<int>();

var differences = from pair in ordered.Zip(ordered.Skip(1))
                  select pair.Second - pair.First;

var part1 = differences.Count(d => d == 1) * differences.Count(d => d == 3);
Debug.Assert(differences.All(d => d == 1 || d == 3));
var part2 = differences.FindNofConsecutiveOnes().Aggregate(1L, (x, y) => x * y);

Console.WriteLine((part1, part2));

static class Ex
{
    internal static IEnumerable<int> FindNofConsecutiveOnes(this IEnumerable<int> differences)
    {
        var tribonnaci = new[] { 1, 1, 2, 4, 7, 13, 24 };
        int consecutiveOnes = 0;
        foreach (var d in differences)
        {
            switch (d)
            {
                case 1:
                    consecutiveOnes++;
                    break;
                case 3:
                    yield return tribonnaci[consecutiveOnes];
                    consecutiveOnes = 0;
                    break;
            }
        }
    }
}