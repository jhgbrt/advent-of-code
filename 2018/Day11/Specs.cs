namespace AdventOfCode.Year2018.Day11;

public class Specs
{
    [Theory]
    [InlineData(3, 5, 8, 4)]
    [InlineData(122, 79, 57, -5)]
    [InlineData(217, 196, 39, 0)]
    [InlineData(101, 153, 71, 4)]
    public void GetCellPowerTest(int x, int y, int serialNumber, int power)
    {
        var result = AoC.GetCellPower(x, y, serialNumber);
        Assert.Equal(power, result);
    }

    [Theory]
    [InlineData(33, 45, 18, 29)]
    public void GetSquarePowerTest(int x, int y, int serialNumber, int expected)
    {
        var result = AoC.GetSquarePower((x, y), serialNumber);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData(18, 33, 45, 29)]
    [InlineData(42, 21, 61, 30)]
    public void TestPart1(int serialNumber, int left, int top, int power)
    {
        var result = AoC.Part1(serialNumber);
        Assert.Equal((left, top, power), result);
    }

    [Theory]
    [InlineData(18, 90, 269, 16, 113)]
    [InlineData(42, 232, 251, 12, 119)]
    public void TestPart2(int serialNumber, int left, int top, int size, int power)
    {
        var result = AoC.Part2(serialNumber);
        Assert.Equal((left, top, size, power), result);
    }
}
