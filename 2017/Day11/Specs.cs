namespace hexgrid
{
    public class Specs
    {
        [Theory]
        [InlineData(2, "ne", "ne")]
        [InlineData(0, "ne", "ne", "sw", "sw")]
        [InlineData(0, "nw", "nw", "se", "se")]
        [InlineData(2, "ne", "ne", "s", "s")]
        [InlineData(2, "nw", "nw", "s", "s")]
        [InlineData(2, "se", "se", "n", "n")]
        [InlineData(2, "sw", "sw", "n", "n")]
        [InlineData(3, "se", "sw", "se", "sw", "sw")]
        [InlineData(3, "ne", "nw", "ne", "nw", "nw")]
        public void Test(int expectedDistance, params string[] steps)
        {
            Assert.Equal(expectedDistance, HexGrid.Calculate(steps).distance);
        }

    }
}