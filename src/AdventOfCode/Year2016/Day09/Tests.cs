namespace AdventOfCode.Year2016.Day09;

public class Tests
{

    [Theory]
    [InlineData("ADVENT", "ADVENT")]
    [InlineData("A(1x5)BC", "ABBBBBC")]
    [InlineData("(3x3)XYZ", "XYZXYZXYZ")]
    [InlineData("(6x1)(1x3)A", "(1x3)A")]
    [InlineData("X(8x2)(3x3)ABCY", "X(3x3)ABC(3x3)ABCY")]
    [InlineData("ADVENTA(1x5)BC(3x3)XYZA(2x2)BCD(2x2)EFG(6x1)(1x3)AX(8x2)(3x3)ABCY",
        "ADVENTABBBBBCXYZXYZXYZABCBCDEFEFG(1x3)AX(3x3)ABC(3x3)ABCY")]
    public void DecompressedLength(string input, string expected)
    {
        var result2 = input.GetDecompressedSize(0);
        Assert.Equal(expected.LongCount(), result2);
    }
    [Theory]
    [InlineData("ADVENT", 6)]
    [InlineData("A(1x5)BC", 7)]
    [InlineData("X(8x2)(3x3)ABCY", 20)]
    [InlineData("(27x12)(20x12)(13x14)(7x10)(1x12)A", 241920)]
    [InlineData("(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN", 445)]
    public void DecompressedLength2(string input, long expected)
    {
        var result2 = input.GetDecompressedSize2(0, input.Length);
        Assert.Equal(expected, result2);
    }
}