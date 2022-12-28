namespace AdventOfCode.Year2020.Day18;



public class Part1Tests
{
    [Fact]
    public void SimpleNumber()
    {
        var result = Part1Calculator.Calculate("123");
        Assert.Equal(123, result);
    }
    [Fact]
    public void SimpleSum()
    {
        var result = Part1Calculator.Calculate("1 + 2");
        Assert.Equal(3, result);
    }
    [Fact]
    public void SimpleProduct()
    {
        var result = Part1Calculator.Calculate("2 * 3");
        Assert.Equal(6, result);
    }
    [Fact]
    public void SimpleProductAndSum()
    {
        var result = Part1Calculator.Calculate("1 + 2 * 3 + 4");
        Assert.Equal(13, result);
    }
    [Fact]
    public void AddBraces()
    {
        var result = Part1Calculator.Calculate("1 + (2 * 3) + 4");
        Assert.Equal(11, result);
    }
    [Fact]
    public void StartWithDoubleBrace()
    {
        var result = Part1Calculator.Calculate("((1 + 5) * (2 * 3) + 4) * 2");
        Assert.Equal(80, result);
    }

    [Fact]
    public void SomeInput()
    {
        var result = Part1Calculator.Calculate("4 * 8 * 9 + (6 * 7 * 8 * (6 * 5 * 2 + 8 + 5)) + 7");
        Assert.Equal(24823, result);
    }

}

public class Part2Tests
{
    [Fact]
    public void SimpleNumber()
    {
        var result = Part2Calculator.Calculate("123");
        Assert.Equal(123, result);
    }
    [Fact]
    public void SimpleSum()
    {
        var result = Part2Calculator.Calculate("1 + 2");
        Assert.Equal(3, result);
    }
    [Fact]
    public void SimpleProduct()
    {
        var result = Part2Calculator.Calculate("2 * 3");
        Assert.Equal(6, result);
    }
    [Fact]
    public void SimpleProductAndSum()
    {
        var result = Part2Calculator.Calculate("1 + 2 * 3 + 4");
        Assert.Equal(21, result);
    }
    [Fact]
    public void AddBraces()
    {
        var result = Part2Calculator.Calculate("1 + (2 * 3) + 4");
        Assert.Equal(11, result);
    }
    [Fact]
    public void StartWithDoubleBrace()
    {
        var result = Part2Calculator.Calculate("((1 + 5) * (2 * 3) + 4) * 2");
        Assert.Equal(120, result);
    }

    [Fact]
    public void SomeInput()
    {
        var result = Part2Calculator.Calculate("4 * 8 * 9 + (6 * 7 * 8 * (6 * 5 * 2 + 8 + 5)) + 7");
        Assert.Equal(4838912, result);
    }

}
