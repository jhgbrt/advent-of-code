using Xunit;

namespace AdventOfCode
{
    public class Specs
    {
        private readonly string input = "dabAcCaCBAcCcaDA";

        [Fact]
        public void TestPart1()
        {
            var result = AoC.Part1(input);
            Assert.Equal(10, result);
        }

        [Fact]
        public void TestPart2()
        {
            var result = AoC.Part2(input);
            Assert.Equal(4, result);
        }
    }
}
