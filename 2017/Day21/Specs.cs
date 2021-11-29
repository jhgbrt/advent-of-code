namespace AdventOfCode.Year2017.Day21;

public class Specs
{
    [Theory]
    [InlineData(".#.\r\n..#\r\n###")]
    [InlineData(".#.\r\n#..\r\n###")]
    [InlineData("#..\r\n#.#\r\n##.")]
    [InlineData("###\r\n..#\r\n.#.")]

    public void IsMatch_ReturnsTrueForSeveralTranspositions(string input)
    {
        var rule = Rule.Parse(".#./..#/### => #..#/..../..../#..#");
        Assert.True(rule.IsMatch(input));
    }

    [Fact]
    public void Squares()
    {
        var input = "ABCD\r\n" +
                    "EFGH\r\n" +
                    "IJKL\r\n" +
                    "MNOP";
        var squares = input.ReadLines().ToRectangular().Squares(2).ToArray();

        Assert.Equal(new[,] { { 'A', 'B' }, { 'E', 'F' } }, squares[0]);
        Assert.Equal(new[,] { { 'I', 'J' }, { 'M', 'N' } }, squares[1]);
        Assert.Equal(new[,] { { 'C', 'D' }, { 'G', 'H' } }, squares[2]);
        Assert.Equal(new[,] { { 'K', 'L' }, { 'O', 'P' } }, squares[3]);
    }

    [Fact]
    public void Assemble()
    {
        var input = "AB\r\n" +
                    "CD";
        var result = string.Join(Environment.NewLine,
            input.ReadLines().ToRectangular().Repeat(2).FromRectangular()
            );

        Assert.Equal("ABAB\r\nCDCD\r\nABAB\r\nCDCD", result);
    }

    [Fact]
    public void Repeat_RepeatsSameSquare()
    {
        var input = ".#.\r\n..#\r\n###".ReadLines().ToRectangular();
        var rules = "../.# => ##./#../...\r\n.#./..#/### => #..#/..../..../#..#".ReadLines().Select(Rule.Parse).ToArray();
        var expected = "##.##.\r\n#..#..\r\n......\r\n##.##.\r\n#..#..\r\n......";

        for (int i = 0; i < 2; i++)
        {
            var factor = input.Length % 2 == 0 ? 2 : 3;
            var match = rules.First(rule => input.Squares(factor).All(rule.IsMatch));
            input = match.Result.Repeat((input.GetUpperBound(0) + 1) / factor);
        }
        Assert.Equal(expected, string.Join(Environment.NewLine, input.FromRectangular()));

    }
    [Fact]
    public void Assemble_LaysOutSquares()
    {
        var s = "ABEF\r\n" +
                "CDGH\r\n" +
                "IJMN\r\n" +
                "KLOP";

        var input = s.ReadLines().ToRectangular().Squares(2);

        var result = input.Assemble(2, 4);

        Assert.Equal(s, string.Join(Environment.NewLine, result.FromRectangular()));

    }

}