using static AdventOfCode.Year2020.Day03.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day03
{
    partial class AoC
    {
        internal static Result Part1() => Run(() => Driver.Part1("input.txt"));
        internal static Result Part2() => Run(() => Driver.Part2("input.txt"));
    }
}

static class Driver
{
    public static long Part1(string input)
    {
        var lines = File.ReadLines(input).ToList();
        var set = lines.GetTrees().ToHashSet();
        return (
            from point in Path((3, 1)).TakeWhile(c => c.y < lines.Count)
            where set.Contains((point.x % lines[0].Length, point.y))
            select point
            ).Count();
    }

    public static IEnumerable<(int x, int y)> Path((int dx, int dy) slope)
    {
        var p = (x: 0, y: 0);
        yield return p;
        while (true)
        {
            yield return p = (p.x + slope.dx, p.y + slope.dy);
        }

    }

    public static IEnumerable<(int x, int y)> GetTrees(this IEnumerable<string> lines)
        => from p in lines.Select((s, i) => (s, i))
           from q in p.s.Trees(p.i)
           select q;

    public static IEnumerable<(int x, int y)> Trees(this string line, int y)
        => from p in line.Select((c, i) => (c, i))
           where p.c == '#'
           select (p.i, y);


    public static long Part2(string input)
    {
        var lines = File.ReadLines(input).ToList();
        var set = lines.GetTrees().ToHashSet();

        var query =
            from slope in new (int dx, int dy)[] { (1, 1), (3, 1), (5, 1), (7, 1), (1, 2) }
            select (
                from point in Path(slope).TakeWhile(c => c.y < lines.Count)
                where set.Contains((point.x % lines[0].Length, point.y))
                select point
            ).Count();

        return query.Aggregate(1L, (x, c) => x * c);
    }
}

namespace AdventOfCode
{
    public class Tests
    {
        ITestOutputHelper _output;

        public Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestPart1()
        {
            Assert.Equal(7, Driver.Part1("sample.txt"));
        }

        [Fact]
        public void GetTreesTest()
        {
            var input =
                "#.#\r\n" +
                ".#.";
            var expected = new[] { (0, 0), (2, 0), (1, 1) };
            var trees = input.Split(Environment.NewLine).GetTrees().ToList();
            Assert.Equal(expected, trees);
        }

        [Fact]
        public void TestPath()
        {
            var p = Driver.Path((3, 1)).Take(4);
            Assert.Equal(new[] { (0, 0), (3, 1), (6, 2), (9, 3) }, p);
        }
        [Fact]
        public void TestPart2()
        {
            var result = Driver.Part2("sample.txt");
            Assert.Equal(336, result);
        }
    }
}
