using System.IO;
using System.Linq;
using Xunit;

namespace firewall
{
    public class Specs
    {
        static string input = "0: 3\r\n" +
                    "1: 2\r\n" +
                    "4: 4\r\n" +
                    "6: 4";
        static (int,int)[] lines = (
            from line in new StringReader(input).ReadLines()
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