namespace ProgramTree
{
    public class Tests
    {
        private string SampleInput = @"pbga (66)
xhth (57)
ebii (61)
havc (66)
ktlj (57)
fwft (72) -> ktlj, cntj, xhth
qoyq (66)
padx (45) -> pbga, havc, qoyq
tknk (41) -> ugml, padx, fwft
jptl (61)
ugml (68) -> gyxo, ebii, jptl
gyxo (61)
cntj (57)
";

        [Fact]
        public void TestPart1()
        {
            var tree = Tree.Parse(SampleInput);
            var root = tree.Root;
            Assert.Equal("tknk", root.Label);
        }

        [Theory]
        [InlineData("ugml", 251)]
        [InlineData("gyxo", 61)]
        [InlineData("padx", 243)]
        public void TestPart2_Weights(string label, int expectedWeight)
        {
            var tree = Tree.Parse(SampleInput);
            var node = tree.Find(label);
            Assert.Equal(expectedWeight, node.Weight);
        }

        [Fact]
        public void TestPart2()
        {
            var tree = Tree.Parse(SampleInput);
            

            var invalidNode =tree.FindInvalidNode();
            var result = invalidNode.RebalancingWeight;

            Assert.Equal("ugml", invalidNode.Label);
            Assert.Equal(60, result);
        }
    }

}