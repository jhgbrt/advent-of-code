using System;
using Xunit;

public class Specs
{
    [Fact]
    public void Test1()
    {
        var result = Spinlock.Find(3, 2017);
        Assert.Equal(638, result.buffer[result.index + 1]);
    }

    [Fact]
    public void Test2()
    {
        var result = Spinlock.Find(3, 2017);
        Assert.Equal(result.buffer[1], Spinlock.FindFast(3, 2017));
    }


}