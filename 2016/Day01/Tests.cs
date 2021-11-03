using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Jeroen.Day1
{

    public class Tests
    {
        private readonly ITestOutputHelper output;

        public Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Theory]
        [InlineData(Bearing.N, Direction.R, Bearing.E)]
        [InlineData(Bearing.E, Direction.R, Bearing.S)]
        [InlineData(Bearing.S, Direction.R, Bearing.W)]
        [InlineData(Bearing.W, Direction.R, Bearing.N)]
        [InlineData(Bearing.N, Direction.L, Bearing.W)]
        [InlineData(Bearing.E, Direction.L, Bearing.N)]
        [InlineData(Bearing.S, Direction.L, Bearing.E)]
        [InlineData(Bearing.W, Direction.L, Bearing.S)]
        public void CompassTests(Bearing current, Direction turn, Bearing expected)
        {
            var compass = new Compass(current);
            compass.Turn(turn);
            Assert.Equal(expected, compass.Bearing);
        }

        [Theory]
        [InlineData("R2, L3", 5)]
        [InlineData("R2, R2, R2", 2)]
        [InlineData("R5, L5, R5, R3", 12)]
        public void Test(string input, int expected)
        {
            var navigator = Navigate(input);
            var result = navigator.Blocks;
            Assert.Equal(expected, result);
        }

        [Theory]
        [InlineData("R1", null)]
        [InlineData("R5, R5, R5, R5", 0)]
        [InlineData("R5, R4, R4, R4, L1", 1)]
        [InlineData("R8, R4, R4, R8", 4)]
        public void FirstPlaceVisitedTwiceTest(string input, int? expected)
        {
            var navigator = Navigate(input);
            var result = navigator.Part2;
            Assert.Equal(expected, result);
        }

        [Fact]
        public void Calculate()
        {
            var input = @"R3, L5, R2, L1, L2, R5, L2, R2, L2, L2, L1, R2, L2, R4, R4, R1, L2, L3, R3, L1, R2, L2, L4, R4, R5, L3, R3, L3, L3, R4, R5, L3, R3, L5, L1, L2, R2, L1, R3, R1, L1, R187, L1, R2, R47, L5, L1, L2, R4, R3, L3, R3, R4, R1, R3, L1, L4, L1, R2, L1, R4, R5, L1, R77, L5, L4, R3, L2, R4, R5, R5, L2, L2, R2, R5, L2, R194, R5, L2, R4, L5, L4, L2, R5, L3, L2, L5, R5, R2, L3, R3, R1, L4, R2, L1, R5, L1, R5, L1, L1, R3, L1, R5, R2, R5, R5, L4, L5, L5, L5, R3, L2, L5, L4, R3, R1, R1, R4, L2, L4, R5, R5, R4, L2, L2, R5, R5, L5, L2, R4, R4, L4, R1, L3, R1, L1, L1, L1, L4, R5, R4, L4, L4, R5, R3, L2, L2, R3, R1, R4, L3, R1, L4, R3, L3, L2, R2, R2, R2, L1, L4, R3, R2, R2, L3, R2, L3, L2, R4, L2, R3, L4, R5, R4, R1, R5, R3";
            var navigator = Navigate(input);
            output.WriteLine(navigator.Blocks.ToString());
            output.WriteLine(navigator.Part2.ToString());
        }

        private Navigator Navigate(string input)
        {
            var navigator = new Navigator();

            var instructions = input.Parse();
            foreach (var (direction, distance) in instructions)
            {
                navigator.Head(direction, distance);
            }

            //foreach (var position in navigator.Path)
            //    output.WriteLine(position.ToString());

            return navigator;
        }

    }
}