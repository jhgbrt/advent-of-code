using System;
using System.Linq;
using Xunit;

namespace AdventOfCode
{
    public class Specs
    {

        [Fact]
        public void Test1()
        {
            string[] input = new[] { "abcdef", "bababc", "abbcde", "abcccd", "aabcdd", "abcdee", "ababab" };
            var result = AoC.Part1(input);
            Assert.Equal(12, result);
        }

        [Fact]
        public void Test2()
        { 
            var input = new[]
            {
                "abcde",
                "fghij",
                "klmno",
                "pqrst",
                "fguij",
                "axcye",
                "wvxyz"
            };
            Assert.Equal("fgij", AoC.Part2(input));
        }

        [Fact]
        public void DiffCount_SimilarStrings()
        {
            Assert.Equal(1, AoC.DiffCount("abcd", "abed"));
        }
        [Fact]
        public void DiffCount_DifferentStrings_All()
        {
            Assert.Equal(4, AoC.DiffCount("abcd", "efgh"));
        }
    }
}

