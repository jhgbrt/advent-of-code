using System.IO;
using Xunit;

public class Tests
{
    [Theory]
    [InlineData("5\t1\t9\t5\r\n" +
                "7\t5\t3\r\n" +
                "2\t4\t6\t8\r\n", 18)]
    public void Test1(string input, int checksum)
    {
        Assert.Equal(checksum, CheckSum.CheckSum1(new StringReader(input)));
    }
    [Theory]
    [InlineData("5\t9\t2\t8\r\n" +
                "9\t4\t7\t3\r\n" +
                "3\t8\t6\t5\r\n", 9)]
    public void Test2(string input, int checksum)
    {
        Assert.Equal(checksum, CheckSum.CheckSum2(new StringReader(input)));
    }
}
