using System;
using System.IO;
using System.Linq;
using Xunit;

namespace AdventOfCode
{
    public class Specs
    {
        string[] testsequence = new[]
        {
            @".#.#...|#.
.....#|##|
.|..|...#.
..|#.....#
#.#|||#|#|
...#.||...
.|....|...
||...#|.#|
|.||||..|.
...#.|..|.",
            @".......##.
......|###
.|..|...#.
..|#||...#
..##||.|#|
...#||||..
||...|||..
|||||.||.|
||||||||||
....||..|.",
            @".......#..
......|#..
.|.|||....
..##|||..#
..###|||#|
...#|||||.
|||||||||.
||||||||||
||||||||||
.|||||||||",
            @".......#..
....|||#..
.|.||||...
..###|||.#
...##|||#|
.||##|||||
||||||||||
||||||||||
||||||||||
||||||||||",
            @".....|.#..
...||||#..
.|.#||||..
..###||||#
...###||#|
|||##|||||
||||||||||
||||||||||
||||||||||
||||||||||",
            @"....|||#..
...||||#..
.|.##||||.
..####|||#
.|.###||#|
|||###||||
||||||||||
||||||||||
||||||||||
||||||||||",
            @"...||||#..
...||||#..
.|.###|||.
..#.##|||#
|||#.##|#|
|||###||||
||||#|||||
||||||||||
||||||||||
||||||||||",
            @"...||||#..
..||#|##..
.|.####||.
||#..##||#
||##.##|#|
|||####|||
|||###||||
||||||||||
||||||||||
||||||||||",
            @"..||||##..
..|#####..
|||#####|.
||#...##|#
||##..###|
||##.###||
|||####|||
||||#|||||
||||||||||
||||||||||",
            @"..||###...
.||#####..
||##...##.
||#....###
|##....##|
||##..###|
||######||
|||###||||
||||||||||
||||||||||",
            @".||##.....
||###.....
||##......
|##.....##
|##.....##
|##....##|
||##.####|
||#####|||
||||#|||||
||||||||||"
        };

        string[] input = new[] 
        {
            @".#.#...|#.",
            @".....#|##|",
            @".|..|...#.",
            @"..|#.....#",
            @"#.#|||#|#|",
            @"...#.||...",
            @".|....|...",
            @"||...#|.#|",
            @"|.||||..|.",
            @"...#.|..|."
        };

        [Fact]
        public void ConstructGrid()
        {
            var grid = new Grid(input);
            Assert.Equal(testsequence[0], grid.ToString());
        }

        [Theory]
        [InlineData(1, 1, '1', '2', '3', '4', '6', '7', '8', '9')]
        [InlineData(0,0,'2','4','5')]
        [InlineData(0,1,'1','2','5', '7', '8')]
        [InlineData(0,2, '4', '5', '8')]
        [InlineData(1,0,'1','3','4', '5', '6')]
        [InlineData(1,2,'4','5','6', '7', '9')]
        [InlineData(2,0,'2','5', '6')]
        public void Surroundings(int x, int y, params char[] expected)
        {
            var grid = new char[3, 3]
            {
                {'1', '2', '3'},
                {'4', '5', '6'},
                {'7', '8', '9'},
            };
            var surroundings = grid.Surroundings(x, y).OrderBy(c => c).ToArray();
            Assert.Equal(expected, surroundings);
        }

        [Fact]
        public void OpenAcre_BecomesFilled()
        {
            // An open acre will become filled with trees if three or more adjacent acres contained trees. 
            // Otherwise, nothing happens.

            var surroundings = new Acre[] { Acre.Tree, Acre.Tree, Acre.Tree };
            var result = Grid.Morph(Acre.Open, surroundings);

            Assert.Equal(Acre.Tree, result);

        }

        [Fact]
        public void Tree_BecomesLumber()
        {
            // An acre filled with trees will become a lumberyard if three or more adjacent acres were lumberyards.
            // Otherwise, nothing happens.
            var surroundings = new Acre[] { Acre.Lumber, Acre.Lumber, Acre.Lumber};
            var result = Grid.Morph(Acre.Tree, surroundings);

            Assert.Equal(Acre.Lumber, result);
        }

        [Fact]
        public void Acre_containing_lumberyard_remains_if_adjacant_to_at_least_one_lumber_and_at_least_one_tree()
        {
            // An acre containing a lumberyard will remain a lumberyard if it was adjacent to at least 
            // one other lumberyard and at least one acre containing trees. Otherwise, it becomes open.
            var surroundings = new Acre[] { Acre.Lumber, Acre.Tree };
            var result = Grid.Morph(Acre.Lumber, surroundings);

            Assert.Equal(Acre.Lumber, result);
        }

        [Fact]
        public void Acre_containing_lumberyard_becomes_open_when_surrounded_by_trees()
        {
            // An acre containing a lumberyard will remain a lumberyard if it was adjacent to at least 
            // one other lumberyard and at least one acre containing trees. Otherwise, it becomes open.
            var surroundings = new Acre[] { Acre.Tree, Acre.Tree, Acre.Tree };
            var result = Grid.Morph(Acre.Lumber, surroundings);

            Assert.Equal(Acre.Open, result);
        }
        [Fact]
        public void Acre_containing_lumberyard_becomes_open_when_surrounded_by_lumber()
        {
            // An acre containing a lumberyard will remain a lumberyard if it was adjacent to at least 
            // one other lumberyard and at least one acre containing trees. Otherwise, it becomes open.
            var surroundings = new Acre[] { Acre.Lumber, Acre.Lumber, Acre.Lumber };
            var result = Grid.Morph(Acre.Lumber, surroundings);

            Assert.Equal(Acre.Open, result);
        }

        [Fact]
        public void Acre_containing_lumberyard_becomes_open_when_surrounded_by_open()
        {
            // An acre containing a lumberyard will remain a lumberyard if it was adjacent to at least 
            // one other lumberyard and at least one acre containing trees. Otherwise, it becomes open.
            var surroundings = new Acre[] { Acre.Open, Acre.Open, Acre.Open };
            var result = Grid.Morph(Acre.Lumber, surroundings);

            Assert.Equal(Acre.Open, result);
        }

        [Theory]
        [InlineData(1,1)]
        [InlineData(2,2)]
        [InlineData(3,3)]
        public void Steps(int steps, int expected)
        {
            var grid = new Grid(input).Step(steps);
            Assert.Equal(testsequence[expected], grid.ToString());
        }

        [Fact]
        public void TestPart1()
        {
            var result = AoC.Part1(input);
            Assert.Equal(1147, result);
        }

        [Fact]
        public void TestPart2()
        {
            var testValue = 1000;
            var input = File.ReadAllLines("input.txt");
            var expected = new Grid(input).Step(testValue).Value;
            var result = AoC.Part2(input);
            Assert.Equal(expected, result);
        }
    }
}
