using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Xunit;
using static System.Math;

public class Specs
{
    [Fact]
    public void Test()
    {
        var input = "0/2\r\n2/2\r\n2/3\r\n3/4\r\n3/5\r\n0/1\r\n10/1\r\n9/10";

        var components = (
            from line in input.ReadLines()
            select Component.Parse(line) into component
            orderby component.Smallest
            select component 
        ).ToImmutableList();

        var strength = Bridge.Strongest(components);
        Assert.Equal(31, strength);
    }
}

