using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Xunit;


var numbers = await "input.txt".LinesToNumbers();

var result1 = Execute(() => numbers.Part1());
var result2 = Execute(() => numbers.Part2());

Console.WriteLine(result1);
Console.WriteLine(result2);

static (T, TimeSpan) Execute<T>(Func<T> f)
{
    var sw = Stopwatch.StartNew();
    var t = f();
    return (t, sw.Elapsed);
}


record Pair(int i, int j) 
{ 
    public int Sum => i + j; 
}
record Triplet(int i, int j, int k) 
{ 
    public int Sum => i + j + k; 
}
static class Driver
{
    public static long Part1(this IEnumerable<int> numbers)
        => (
            from p in numbers.GetPairs()
            where p.Sum == 2020
            select p.i
            ).Distinct().Aggregate(1L, (i, m) => m * i);

    public static long Part2(this IEnumerable<int> numbers)
        => (
            from p in numbers.GetTriplets()
            where p.Sum == 2020
            select p.i
            ).Distinct().Aggregate(1L, (i, m) => m * i);

    public static async Task<IEnumerable<int>> LinesToNumbers(this string filename) 
        => from line in await File.ReadAllLinesAsync(filename)
           select int.Parse(line);

    public static IEnumerable<Pair> GetPairs(this IEnumerable<int> numbers) 
        => from i in numbers
           from j in numbers
           select new Pair(i, j);
    
    public static IEnumerable<Triplet> GetTriplets(this IEnumerable<int> numbers) 
        => from i in numbers
           from j in numbers
           from k in numbers
           select new Triplet(i, j, k);
}

namespace AdventOfCode
{
    public class Tests
    {
        [Fact]
        async Task TestPart1()
        {
            var numbers = (await "example.txt".LinesToNumbers()).ToList();
            var result = numbers.Part1();
            Assert.Equal(514579, result);
        }

        [Fact]
        public async Task AllNumbersAreUnique()
        {
            var numbers = (await "input.txt".LinesToNumbers()).ToList();
            Assert.Equal(numbers.Count, numbers.Distinct().Count());
        }

        [Fact]
        async Task TestPart2()
        {
            var numbers = (await "example.txt".LinesToNumbers()).ToList();
            var result = numbers.Part2();
            Assert.Equal(241861950, result);
        }
    }
}
