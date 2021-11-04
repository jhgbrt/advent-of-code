using System.Diagnostics;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    public static object Part1() => input.Where(IsNice1).Count();
    public static object Part2() => input.Where(IsNice2).Count();

    public static bool IsNice1(string s)
    {
        var forbidden = new[] { ('a', 'b'), ('c', 'd'), ('p', 'q'), ('x', 'y') };
        var aggregate = s.Aggregate((vowels: 0, consecutives: 0, previous: '\0', forbidden: false)
            , (acc, c) => (
                vowels: acc.vowels + (IsVowel(c) ? 1 : 0),
                consecutives: acc.consecutives + (c == acc.previous ? 1 : 0),
                previous: c,
                forbidden: acc.forbidden || forbidden.Contains((acc.previous, c))
            ));
        return aggregate.vowels >= 3 && aggregate.consecutives >= 1 && !aggregate.forbidden;
    }

    public static bool IsNice2(string s)
    {
        return ContainsNonOverlappingDoublePair(s) && HasOneRepeatingLetterWithinOneSpace(s);
    }
    public static bool HasOneRepeatingLetterWithinOneSpace(string s)
    {
        for (var i = 0; i < s.Length - 2; i++)
        {
            if (s[i] == s[i + 2]) return true;
        }
        return false;
    }
    public static bool ContainsNonOverlappingDoublePair(string s)
    {
        for (var i = 0; i < s.Length - 3; i++)
            for (var j = i + 2; j < s.Length - 1; j++)
                if (((s[i], s[i + 1]) == (s[j], s[j + 1])))
                    return true;
        return false;
    }

    public static bool IsVowel(char c) => c == 'a' || c == 'e' || c == 'i' || c == 'o' || c == 'u';
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(258, Part1());
    [Fact]
    public void Test2() => Assert.Equal(53, Part2());

    [Theory]
    [InlineData("ugknbfddgicrmopn", true)]
    [InlineData("aaa", true)]
    [InlineData("jchzalrnumimnmhp", false)]
    [InlineData("haegwjzuvuyypxyu", false)]
    [InlineData("dvszwmarrgswjxmb", false)]
    public void IsNice1Tests(string input, bool expected) => Assert.Equal(expected, IsNice1(input));

    [Theory]
    [InlineData("qjhvhtzxzqqjkmpb", true)]
    [InlineData("xxyxx", true)]
    [InlineData("uurcxstgmygtbstg", false)]
    [InlineData("uurcxcstgmygtbstg", true)]
    [InlineData("ieodomkazucvgmuy", false)]
    public void IsNice2Tests(string input, bool expected) => Assert.Equal(expected, IsNice2(input));

}



