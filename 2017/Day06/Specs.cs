using System.Collections.Immutable;

public class Specs
{
    [Fact]
    public void CycleTest_WhenLargestValueIsEvenlyDivisable()
    {
        Assert.Equal(new byte[] { 2, 2, 2, 2 }, Memory.DoOneCycle(new byte[] { 0, 0, 0, 8 }.ToImmutableArray()));
    }
    [Fact]
    public void CycleTest_WhenLargestValueIsNotEvenlyDivisable()
    {
        Assert.Equal(new byte[] { 2, 2, 2, 1 }, Memory.DoOneCycle(new byte[] { 0, 0, 0, 7 }.ToImmutableArray()));
    }
    [Fact]
    public void CycleTest_WhenLargestValueIsNotEvenlyDivisable_AndMaxItemNotLast()
    {
        Assert.Equal(new byte[] { 2, 2, 1, 2 }, Memory.DoOneCycle(new byte[] { 0, 0, 7, 0 }.ToImmutableArray()));
    }
    [Fact]
    public void CycleTest_Step1()
    {
        Assert.Equal(new byte[] { 2, 4, 1, 2 }, Memory.DoOneCycle(new byte[] { 0, 2, 7, 0 }.ToImmutableArray()));
    }
    [Fact]
    public void CycleTest_Step2()
    {
        Assert.Equal(new byte[] { 3, 1, 2, 3 }, Memory.DoOneCycle(new byte[] { 2, 4, 1, 2 }.ToImmutableArray()));
    }
    [Fact]
    public void CycleTest_Step3()
    {
        Assert.Equal(new byte[] { 0, 2, 3, 4 }, Memory.DoOneCycle(new byte[] { 3, 1, 2, 3 }.ToImmutableArray()));
    }
    [Fact]
    public void CycleTest_Step4()
    {
        Assert.Equal(new byte[] { 1, 3, 4, 1 }, Memory.DoOneCycle(new byte[] { 0, 2, 3, 4 }.ToImmutableArray()));
    }
    [Fact]
    public void CycleTest_Step5()
    {
        Assert.Equal(new byte[] { 2, 4, 1, 2 }, Memory.DoOneCycle(new byte[] { 1, 3, 4, 1 }.ToImmutableArray()));
    }

    [Theory]
    [InlineData(5, 4, (byte)0, (byte)2, (byte)7, (byte)0)]
    public void Example(int expectedSteps, int expectedLoopSize, params byte[] input)
    {
        var result = Memory.Cycles(input.ToImmutableArray());
        Assert.Equal(expectedSteps, result.steps);
        Assert.Equal(expectedLoopSize, result.loopSize);
    }

    [Fact]
    public void FindMax_FindsLargestElement()
    {
        var input = new byte[] { 0, 2, 1 };
        var max = input.FindMax();
        Assert.Equal((1, 2), max);
    }

    [Fact]
    public void FindMax_TiesWonByLowestIndex()
    {
        var input = new byte[] { 0, 0 };
        var max = input.FindMax();
        Assert.Equal(0, max.index);
    }
}
