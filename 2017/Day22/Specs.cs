public class Specs
{
    [Fact]
    public void Test1()
    {
        var grid = "..#\r\n#..\r\n...".ReadLines().ToRectangular();
        var result = new Grid(grid).InfectGrid(10000);
        Assert.Equal(5587, result);
    }
    [Fact]
    public void Test2()
    {
        var grid = "..#\r\n#..\r\n...".ReadLines().ToRectangular();
        var result = new Grid(grid).InfectGrid2(100);
        Assert.Equal(26, result);
    }
}