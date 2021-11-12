namespace AdventOfCode
{
    public class Specs
    {
        [Theory]
        [InlineData("#123 @ 3,2: 5x4")]
        public void ToRectangle(string input)
        {
            var (left, top, width, height, id) = AoC.ToRectangle(input);
            Assert.Equal(3, left);
            Assert.Equal(2, top);
            Assert.Equal(5, width);
            Assert.Equal(4, height);
        }

        string[] input = new[]
        {
                "#1 @ 1,3: 4x4",
                "#2 @ 3,1: 4x4",
                "#3 @ 5,5: 2x2",
            };

        [Fact]
        public void TestPart1()
        {
            var result = AoC.Part1(input);
            Assert.Equal(4, result);
        }


        [Fact]
        public void TestPart2()
        {
            var result = AoC.Part2(input);
            Assert.Equal(3, result);
        }
    }
}
