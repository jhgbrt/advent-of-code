using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AdventOfCode
{
    public class Specs
    {
        string[] input = new[] { "" };

        [Fact]
        public void GetDigits()
        {
            Assert.Equal(new[] { 0 }, 0.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 1 }, 1.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 8 }, 8.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 9 }, 9.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 1, 0 }, 10.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 1, 0 }, 10.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 1, 1 }, 11.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 1, 2 }, 12.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 1, 3 }, 13.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 1, 4 }, 14.GetDigits().Reverse().ToArray());
            Assert.Equal(new[] { 1, 9 }, 19.GetDigits().Reverse().ToArray());
        }

        [Theory]
        [InlineData(9, 5158916779)]
        [InlineData(5, 0124515891)]
        [InlineData(18, 9251071085)]
        [InlineData(2018, 5941429882)]
        public void Test(int n, long expected)
        {
            var recipes = new List<int> { 3, 7 };
            int i = 0;
            int j = 1;
            while (recipes.Count < (n + 10))
            {
                var sum = recipes[i] + recipes[j];
                recipes.AddRange(sum.GetDigits().Reverse());
                i = (i + 1 + recipes[i]) % recipes.Count;
                j = (j + 1 + recipes[j]) % recipes.Count;
            }
            long result = 1000000000L * recipes[n - 1 + 1]
                       + 100000000L * recipes[n - 1 + 2]
                       + 10000000L * recipes[n - 1 + 3]
                       + 1000000L * recipes[n - 1 + 4]
                       + 100000L * recipes[n - 1 + 5]
                       + 10000L * recipes[n - 1 + 6]
                       + 1000L * recipes[n - 1 + 7]
                       + 100L * recipes[n - 1 + 8]
                       + 10L * recipes[n - 1 + 9]
                       + 1L * recipes[n - 1 + 10];
            Assert.Equal(expected, result);
        }


        [Theory]
        [InlineData(9, 5158916779)]
        [InlineData(5, 0124515891)]
        [InlineData(18, 9251071085)]
        [InlineData(2018, 5941429882)]
        public void TestPart1(int n, long expected)
        {
            var result = AoC.Part1(n);
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData(9, 51589)]
        [InlineData(5, 01245)]
        [InlineData(18, 92510)]
        [InlineData(2018, 59414)]
        public void TestPart2(int expected, int input)
        {
            var result = AoC.Part2(input);
            Assert.Equal(expected, result);
        }
    }
}
