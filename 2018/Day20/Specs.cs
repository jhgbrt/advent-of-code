namespace AdventOfCode.Year2018.Day20;

public class Specs
{
    string[] input = new[] { "^WNE$" };


    [Theory]
    [InlineData(3, "^WNE$")]
    [InlineData(10, "^ENWWW(NEEE|SSE(EE|N))$")]
    [InlineData(18, "^ENNWSWW(NEWS|)SSSEEN(WNSE|)EE(SWEN|)NNN$")]
    [InlineData(23, "^ESSWWN(E|NNENN(EESS(WNSE|)SSS|WWWSSSSE(SW|NNNE)))$")]
    [InlineData(31, "^WSSEESWWWNW(S|NENNEEEENN(ESSSSW(NWSW|SSEN)|WSWWN(E|WWS(E|SS))))$")]
    public void ShortestPathTest(int expected, string route)
    {
        Assert.Equal(expected, route.Distances().Max());
    }

    [Fact]
    public void TestPart1()
    {
        var result = AoC.Part1(input);
    }

    [Fact]
    public void TestPart2()
    {
        var result = AoC.Part2(input);
    }
}
