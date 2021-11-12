namespace AdventOfCode
{
    public class Specs
    {
        string input = "2 3 0 3 10 11 12 1 1 0 1 99 2 1 1 2";

        [Fact]
        public void TestPart1()
        {
            Assert.Equal(2, input.ToIntegers().Last());
            var result = AoC.Part1(input);
            Assert.Equal(138, result);
        }

        [Fact]
        public void Node_Value_WhenNoChildren_IsSumOfMetadata()
        {
            var node = new Node(new Node[0], new [] { 1, 2, 3 });
            Assert.Equal(6, node.GetValue());
        }

        [Fact]
        public void Node_Value_WithChildren_IsSumOfValidChildValues()
        {
            var children = new[]
            {
                new Node(new Node[0], new [] { 6 }),
                new Node(new Node[0], new [] { 5 }),
            };
            var parent = new Node(children, new[] { 0, 1, 1, 2, 3 }); // 0 and 3 should be ignored
            Assert.Equal(17, parent.GetValue());
        }

        [Fact]
        public void TestPart2()
        {
            var result = AoC.Part2(input);
            Assert.Equal(66, result);
        }
    }
}
