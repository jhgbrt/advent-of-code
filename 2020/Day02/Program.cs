using static AdventOfCode.Year2020.Day02.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day02
{
    partial class AoC
    {
        internal static Result Part1() => Run(() => Driver.Part1("input.txt"));
        internal static Result Part2() => Run(() => Driver.Part2("input.txt"));
    }
}

record Entry(int Min, int Max, char Letter, string Password);

static class Driver
{
    public static Regex LineRegex = new(@"(?<Min>\d+)-(?<Max>\d+) (?<Letter>\w): (?<Password>\w+)", RegexOptions.Compiled);

    public static bool IsValid1(Entry entry)
    {
        var n = entry.Password.Count(x => x == entry.Letter);
        return n >= entry.Min && n <= entry.Max;
    }

    public static bool IsValid2(Entry entry)
    {
        return entry.Password[entry.Min - 1] == entry.Letter ^ entry.Password[entry.Max - 1] == entry.Letter;
    }

    public static Entry ToEntry(string input)
    {
        var match = LineRegex.Match(input);
        var groups = match.Groups;
        return new Entry(
            int.Parse(groups["Min"].Value),
            int.Parse(groups["Max"].Value),
            groups["Letter"].Value.Single(),
            groups["Password"].Value
            );
    }

    public static long Part1(string input)
        => File.ReadLines(input)
        .Select(ToEntry)
        .Where(e => IsValid1(e))
        .Count();

    public static long Part2(string input)
        => File.ReadLines(input)
        .Select(ToEntry)
        .Where(e => IsValid2(e))
        .Count();
}

namespace AdventOfCode
{
    public class Tests
    {
        [Fact]
        public void TestRegex()
        {
            var matches = Driver.LineRegex.Match("123-456 x: asdfasdf");
            Assert.True(matches.Success);
            Assert.Equal("123", matches.Groups["Min"].Value);
            Assert.Equal("456", matches.Groups["Max"].Value);
            Assert.Equal("x", matches.Groups["Letter"].Value);
            Assert.Equal("asdfasdf", matches.Groups["Password"].Value);
        }

        [Theory]
        [InlineData("1-3 a: abcde", true)]
        [InlineData("1-3 a: aaaaa", false)]
        [InlineData("1-3 a: bcde", false)]
        public void Rule1Test(string input, bool expected)
        {
            Assert.Equal(expected, Driver.IsValid1(Driver.ToEntry(input)));
        }
        [Theory]
        [InlineData("1-3 a: abcde", true)]
        [InlineData("1-3 a: cbade", true)]
        [InlineData("1-3 a: ddddd", false)]
        [InlineData("1-3 c: ccccc", false)]
        public void Rule2Test(string input, bool expected)
        {
            Assert.Equal(expected, Driver.IsValid1(Driver.ToEntry(input)));
        }

        [Fact]
        public void TestPart1()
        {
            var result = Driver.Part1("sample.txt");
            Assert.Equal(2, result);
        }

        [Fact]
        public void TestPart2()
        {
            var result = Driver.Part2("sample.txt");
            Assert.Equal(1, result);
        }
    }
}
