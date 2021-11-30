namespace AdventOfCode.Year2018.Day12;

public class Specs
{
    string _initialState = "#..#.#..##......###...###";
    string[] input = new[]
    {
            "initial state: #..#.#..##......###...###",
            "",
            "...## => #",
            "..#.. => #",
            ".#... => #",
            ".#.#. => #",
            ".#.## => #",
            ".##.. => #",
            ".#### => #",
            "#.#.# => #",
            "#.### => #",
            "##.#. => #",
            "##.## => #",
            "###.. => #",
            "###.# => #",
            "####. => #"
        };


    [Theory]
    [InlineData(1, "...#...#....#.....#..#..#..#...........")]
    [InlineData(2, "...##..##...##....#..#..#..##..........")]
    [InlineData(3, "..#.#...#..#.#....#..#..#...#..........")]
    [InlineData(4, "...#.#..#...#.#...#..#..##..##.........")]
    [InlineData(5, "....#...##...#.#..#..#...#...#.........")]
    [InlineData(6, "....##.#.#....#...#..##..##..##........")]
    [InlineData(7, "...#..###.#...##..#...#...#...#........")]
    [InlineData(8, "...#....##.#.#.#..##..##..##..##.......")]
    [InlineData(9, "...##..#..#####....#...#...#...#.......")]
    [InlineData(10, "..#.#..#...#.##....##..##..##..##......")]
    [InlineData(11, "...#...##...#.#...#.#...#...#...#......")]
    [InlineData(12, "...##.#.#....#.#...#.#..##..##..##.....")]
    [InlineData(13, "..#..###.#....#.#...#....#...#...#.....")]
    [InlineData(14, "..#....##.#....#.#..##...##..##..##....")]
    [InlineData(15, "..##..#..#.#....#....#..#.#...#...#....")]
    [InlineData(16, ".#.#..#...#.#...##...#...#.#..##..##...")]
    [InlineData(17, "..#...##...#.#.#.#...##...#....#...#...")]
    [InlineData(18, "..##.#.#....#####.#.#.#...##...##..##..")]
    [InlineData(19, ".#..###.#..#.#.#######.#.#.#..#.#...#..")]
    [InlineData(20, ".#....##....#####...#######....#.#..##.")]
    public void TransformTest(int generation, string expected)
    {
        var rules = input.Skip(2).Select(line => line.Split(" => ")).Select(c => (c[0], c[1][0])).ToArray();
        string result = _initialState;
        var zero = 0;
        for (int i = 0; i < generation; i++)
        {
            if (result[0..5].Contains('#') || result[^5..^0].Contains('#'))
            {
                result = "....." + result + ".....";
                zero += 5;
            }
            result = AoC201812.Transform(result, rules);
        }
        result = result.Substring(result.IndexOf('#'));
        expected = expected.Substring(expected.IndexOf('#'));
        var minLength = Math.Min(result.Length, expected.Length);
        Assert.Equal(new string(expected.Take(minLength).ToArray()), new string(result.Take(minLength).ToArray()));
    }


    [Fact]
    public void TestPart1()
    {
        var result = AoC201812.Part1(input, 20);
        Assert.Equal(325, result);
    }

    [Fact]
    public void TestPart2()
    {
        //var result = AoCImpl.Part2(input);
    }
}
