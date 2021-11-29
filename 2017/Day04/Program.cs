using static AdventOfCode.Year2017.Day04.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2017.Day04
{
    partial class AoC
    {
        static bool test = false;
        public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

        internal static Result Part1() => Run(() => input.Count(IsValidPassword1));
        internal static Result Part2() => Run(() => input.Count(IsValidPassword2));

        private static bool IsValidPassword1(string line)
        {
            var words = line.Split(' ');
            return words.Length == words.Distinct().Count();
        }
        private static bool IsValidPassword2(string line)
        {
            var words = line.Split(' ').Select(w => new string(w.OrderBy(c => c).ToArray())).ToArray();
            return words.Length == words.Distinct().Count();
        }

    }
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(451, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(223, Part2().Value);
}


