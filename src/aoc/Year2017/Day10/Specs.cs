namespace AdventOfCode.Year2017.Day10;

public class Specs
{
    [Fact]
    public void CircularSlice_ReturnsExpectedValues()
    {
        var input = new byte[] { 0, 1, 2, 3, 4 };
        Assert.Equal(new byte[] { 3, 4, 0, 1 }, input.CircularSlice(3, 4).ToArray());
    }

    private static readonly byte[] Lengths = { 3, 4, 1, 5 };
    [Theory]
    [InlineData(1, 2, 1, 0, 3, 4)]
    [InlineData(2, 4, 3, 0, 1, 2)]
    [InlineData(3, 4, 3, 0, 1, 2)]
    [InlineData(4, 3, 4, 2, 1, 0)]
    public void TestHash(int steps, params int[] expected)
    {
        var result = KnotsHash.Hash(Lengths.Take(steps).ToArray(), 5);
        Assert.Equal(expected, result.Select(b => (int)b));
    }

    [Fact]
    public void ReduceTest()
    {
        var sparseHash = new byte[]
        {
                65, 27, 9, 1, 4, 3, 40, 50, 91, 7, 6, 0, 2, 5, 68, 22,
                65, 27, 9, 1, 4, 3, 40, 50, 91, 7, 6, 0, 2, 5, 68, 22
        };
        Assert.Equal(new byte[] { 64, 64 }, sparseHash.ReduceHash());
    }

    [Theory]
    [InlineData("", "a2582a3a0e66e6e86e3812dcb672a272")]
    [InlineData("AoC 2017", "33efeb34ea91902bb2f59c9920caa6cd")]
    [InlineData("1,2,3", "3efbe78a8d82f29979031a4aa0b16a9d")]
    [InlineData("1,2,4", "63960835bcdc130f0b66d7ff4f6a5a8e")]
    public void HashStringPart2(string input, string expected)
    {
        var result = KnotsHash.Hash(input);
        Assert.Equal(expected, result);
    }
}
