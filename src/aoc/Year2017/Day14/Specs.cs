namespace AdventOfCode.Year2017.Day14;

public class Specs
{
    [Fact]
    public void HashToBits()
    {
        var x = 0xa0c20170;
        Assert.Equal(9, Defrag.CountBits(x));
    }

    [Fact]
    public void Example()
    {
        Assert.Equal(8108, Defrag.CountBitsInGrid("flqrgnkx"));
    }

    [Fact]
    public void Regions()
    {
        Assert.Equal(1242, Defrag.CountRegions("flqrgnkx"));
    }
}
