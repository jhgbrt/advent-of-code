
using static AdventOfCode.Year2016.Day06.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2016.Day06
{
    partial class AoC
    {
        static bool test = false;
        static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

        internal static Result Part1() => Run(() => new Accumulator().Decode(input, 8, false));
        internal static Result Part2() => Run(() => new Accumulator().Decode(input, 8, true));
    }

    public class Tests
    {
        static string[] input = File.ReadAllLines("sample.txt");
        [Fact]
        public void Test1() => Assert.Equal("kjxfwkdh", Part1().Value);
        [Fact]
        public void Test2() => Assert.Equal("xrwcsnps", Part2().Value);
        [Fact]
        public void Part1Test()
        {
            var result = new Accumulator().Decode(input, 6);
            Assert.Equal("easter", result);
        }

        [Fact]
        public void Part2Test()
        {
            var result = new Accumulator().Decode(input, 6, true);
            Assert.Equal("advent", result);
        }
    }




    public class Accumulator
    {
        public string Decode(IEnumerable<string> data, int lineLength, bool ascending = false)
        {
            var query = from line in data
                        from item in line.Select((c, i) => new { c, pos = i })
                        select item;

            var lookup = query.ToLookup(item => item.pos);

            var sb = new StringBuilder();
            for (int i = 0; i < lineLength; i++)
            {
                var g = lookup[i];
                var grpByChar = g.GroupBy(item => item.c);
                var ordered = ascending
                    ? grpByChar.OrderBy(x => x.Count())
                    : grpByChar.OrderByDescending(x => x.Count());
                var c = ordered.First().First().c;
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}