namespace AdventOfCode.Year2017.Day02;

public class Tests
{

    [Theory]
    [InlineData("5\t1\t9\t5\r\n" +
                "7\t5\t3\r\n" +
                "2\t4\t6\t8\r\n", 18)]
    public void TestPart1(string input, int checksum)
    {
        Assert.Equal(checksum, AoC201702.CheckSum1(new StringReader(input)));
    }
    [Theory]
    [InlineData("5\t9\t2\t8\r\n" +
                "9\t4\t7\t3\r\n" +
                "3\t8\t6\t5\r\n", 9)]
    public void TestPart2(string input, int checksum)
    {
        Assert.Equal(checksum, AoC201702.CheckSum2(new StringReader(input)));
    }
}