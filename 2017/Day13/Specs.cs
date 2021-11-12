namespace firewall
{
    public class Specs
    {
        static string[] input = new[] {"0: 3",
                    "1: 2",
                    "4: 4",
                    "6: 4" };
        static (int,int)[] lines = (
            from line in input
            let indexes = line.Split(": ").Select(int.Parse).ToArray()
            select (layer: indexes[0], range: indexes[1])
        ).ToArray();

        [Fact]
        public void Example()
        {
            Assert.Equal(24, Firewall.Severity(lines));
        }

        [Fact]
        public void DelayTest()
        {
            Assert.Equal(10, Firewall.DelayToEscape(lines));
        }
    }
}