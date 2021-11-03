using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Jeroen
{

    public class Tests
    {
        ITestOutputHelper output;

        public Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public void GetRow()
        {
            int[,] x = 
            {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };
            
            Assert.Equal(new[]{11,12,13,14,15}, x.Row(0));
        }

        [Fact]
        public void GetColumn()
        {
            int[,] x =
            {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

            Assert.Equal(new[] { 12, 22, 32, 42 }, x.Column(1));
        }

        [Fact]
        public void RotateRight()
        {
            int[,] x =
            {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

            Assert.Equal(new[] { 14, 15, 11, 12, 13}, x.Row(0).ToList().Rotate(2));
        }

        [Fact]
        public void RotateColumn()
        {
            int[,] x =
            {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
            };
            x.RotateCol(1, 1);
            Assert.Equal(new[,] {
                {11, 32, 13, 14, 15},
                {21, 12, 23, 24, 25},
                {31, 22, 33, 34, 35},
            }, x);
        }

        [Fact]
        public void ReplaceRow()
        {
            int[,] x =
            {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

            x.ReplaceRow(2, new[] { 1, 2, 3, 4, 5 });

            Assert.Equal(new[,] {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {1, 2, 3, 4, 5},
                {41, 42, 43, 44, 45}
            }, x);
        }
        [Fact]
        public void ReplaceCol()
        {
            int[,] x =
            {
                {11, 12, 13, 14, 15},
                {21, 22, 23, 24, 25},
                {31, 32, 33, 34, 35},
                {41, 42, 43, 44, 45}
            };

            x.ReplaceCol(3, new[] { 1, 2, 3, 4 });

            Assert.Equal(new[,] {
                {11, 12, 13, 1, 15},
                {21, 22, 23, 2, 25},
                {31, 32, 33, 3, 35},
                {41, 42, 43, 4, 45}
            }, x);
        }

        [Fact]
        public void RegexRotate()
        {
            Regex rotate = new Regex("rotate (?<op>(row|column)) (x|y)=(?<i>\\d*) by (?<by>\\d*)", RegexOptions.Compiled);
            var match = rotate.Match("rotate row y=7 by 20");
            Assert.Equal("7", match.Groups["i"].Value);
            Assert.Equal("20", match.Groups["by"].Value);
            Assert.Equal("row", match.Groups["op"].Value);
        }
    }
}
