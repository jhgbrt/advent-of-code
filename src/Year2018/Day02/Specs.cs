namespace AdventOfCode.Year2018.Day02;

public class Specs
{

    [Fact]
    public void Test1()
    {
        string[] input = new[] { "abcdef", "bababc", "abbcde", "abcccd", "aabcdd", "abcdee", "ababab" };
        var result = AoCImpl.Part1(input);
        Assert.Equal(12, result);
    }

    [Fact]
    public void Test2()
    {
        var input = new[]
        {
                "abcde",
                "fghij",
                "klmno",
                "pqrst",
                "fguij",
                "axcye",
                "wvxyz"
            };
        Assert.Equal("fgij", AoCImpl.Part2(input));
    }

    [Fact]
    public void DiffCount_SimilarStrings()
    {
        Assert.Equal(1, AoCImpl.DiffCount("abcd", "abed"));
    }
    [Fact]
    public void DiffCount_DifferentStrings_All()
    {
        Assert.Equal(4, AoCImpl.DiffCount("abcd", "efgh"));
    }
}

