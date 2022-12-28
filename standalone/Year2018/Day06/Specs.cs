namespace AdventOfCode.Year2018.Day06;

public class Specs
{
    private readonly string[] input = new[]
    {
             "1, 1"
            ,"1, 6"
            ,"8, 3"
            ,"3, 4"
            ,"5, 5"
            ,"8, 9"
         };

    [Fact]
    public void ToCoordinate()
    {
        var input = "123, 456";
        var coordinate = input.ToCoordinate();
        Assert.Equal(123, coordinate.x);
        Assert.Equal(456, coordinate.y);
    }

    [Fact]
    public void TestPart1()
    {
        var result = AoC201806.Part1(input);
        Assert.Equal(17, result);
    }

    [Fact]
    public void TestPart2()
    {
        var result = AoC201806.Part2(input, 32);
        Assert.Equal(16, result);
    }
}
