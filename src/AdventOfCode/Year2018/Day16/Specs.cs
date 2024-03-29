namespace AdventOfCode.Year2018.Day16;

public class Specs
{
    
    [Fact]
    public void addr_adds_register_0_and_register_1()
    {
        var input = new[] { 5, 3, 1, 0 };
        OpCode.addr(input, 0, 1, 2, 3);
        Assert.Equal(new[] { 5, 3, 1, 4 }, input);
    }
    [Fact]
    public void addi_adds_register_0_and_value_()
    {
        var input = new[] { 5, 3, 1, 0 };
        OpCode.addi(input, 0, 1, 2, 3);
        Assert.Equal(new[] { 5, 3, 1, 5 }, input);
    }
}
