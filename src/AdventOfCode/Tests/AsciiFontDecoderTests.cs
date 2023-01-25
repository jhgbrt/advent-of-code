namespace AdventOfCode.Tests;

public class AsciiFontDecoderTests
{
    ITestOutputHelper _output;

    public AsciiFontDecoderTests(ITestOutputHelper output)
    {
        _output = output;
    }
    const string Alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    const string Alphabet6x10 = """
          xx   xxxxx   xxxx  xxxxx  xxxxxx xxxxxx  xxxxx x    x xxxxx     xxx x    x x      x    x x    x  xxxx  xxxxx   xxxx  xxxxx   xxxx  xxxxxx x    x x    x x    x x    x x    x xxxxxx 
         x  x  x    x x    x x    x x      x      x      x    x   x        x  x   x  x      xx  xx x    x x    x x    x x    x x    x x        x    x    x x    x x    x x    x x    x      x 
        x    x x    x x      x    x x      x      x      x    x   x        x  x  x   x      x xx x xx   x x    x x    x x    x x    x x        x    x    x x    x x    x  x  x   x  x       x 
        x    x x    x x      x    x x      x      x      x    x   x        x  x x    x      x    x xx   x x    x x    x x    x x    x x        x    x    x x    x x    x  x  x   x  x      x  
        x    x xxxxx  x      x    x xxxxx  xxxxx  x      xxxxxx   x        x  xx     x      x    x x x  x x    x xxxxx  x    x xxxxx   xxxx    x    x    x  x  x  x    x   xx     xx      x   
        xxxxxx x    x x      x    x x      x      x  xxx x    x   x        x  xx     x      x    x x  x x x    x x      x    x x  x        x   x    x    x  x  x  x    x   xx      x     x    
        x    x x    x x      x    x x      x      x    x x    x   x        x  x x    x      x    x x   xx x    x x      x    x x   x       x   x    x    x  x  x  x    x  x  x     x    x     
        x    x x    x x      x    x x      x      x    x x    x   x    x   x  x  x   x      x    x x   xx x    x x      x    x x   x       x   x    x    x  x  x  x xx x  x  x     x   x      
        x    x x    x x    x x    x x      x      x    x x    x   x    x   x  x   x  x      x    x x    x x    x x      x   x  x    x      x   x    x    x   xx   xx  xx x    x    x   x      
        x    x xxxxx   xxxx  xxxxx  xxxxxx x       xxxx  x    x xxxxx   xxx   x    x xxxxxx x    x x    x  xxxx  x       xxx x x    x xxxxx    x     xxxx    xx   x    x x    x   x    xxxxxx 
        """;
    const string Alphabet4x6 = """
         xx  xxx   xx  xxx  xxxx xxxx  xx  x  x xxx    xx x  x x    x  x x  x  xx  xxx   xx  xxx   xxx xxxx x  x x  x x  x x  x x  x xxxx 
        x  x x  x x  x x  x x    x    x  x x  x  x      x x x  x    xxxx xx x x  x x  x x  x x  x x     x   x  x x  x x  x x  x x  x    x 
        x  x xxx  x    x  x xxx  xxx  x    xxxx  x      x xx   x    x  x xx x x  x x  x x  x x  x x     x   x  x x  x x  x  xx   xx    x  
        xxxx x  x x    x  x x    x    x xx x  x  x      x xx   x    x  x x xx x  x xxx  x  x xxx   xx   x   x  x  xx  x  x x  x   x   x   
        x  x x  x x  x x  x x    x    x  x x  x  x   x  x x x  x    x  x x xx x  x x    x xx x x     x  x   x  x  xx  xxxx x  x   x  x    
        x  x xxx   xx  xxx  xxxx x     xxx x  x xxx   xx  x  x xxxx x  x x  x  xx  x     xxx x  x xxx   x    xx   xx  x  x x  x  x   xxxx 
        """;
    [Theory]
    [InlineData(Alphabet6x10, Alphabet, AsciiFontSize._6x10)]
    [InlineData(Alphabet4x6, Alphabet, AsciiFontSize._4x6)]
    public void Test(string input, string expected, AsciiFontSize size)
    {
        var decoder = AsciiFonts.GetFont(size, 'x', ' ');
        Assert.Equal(expected, decoder.Decode(input));
    }
    [Theory]
    [InlineData(Alphabet, Alphabet6x10, AsciiFontSize._6x10)]
    [InlineData(Alphabet, Alphabet4x6, AsciiFontSize._4x6)]
    public void TestEncode(string input, string expected, AsciiFontSize size)
    {
        var decoder = AsciiFonts.GetFont(size, 'x', ' ');
        Assert.Equal(expected + Environment.NewLine, decoder.Encode(input));
    }

    [Fact]
    public void TrailingNewline()
    {
        var input = """
            ###...##..#..#.####.###..#....###..###.
            #..#.#..#.#..#.#....#..#.#....#..#.#..#
            #..#.#....#..#.###..#..#.#....#..#.#..#
            ###..#.##.#..#.#....###..#....###..###.
            #....#..#.#..#.#....#....#....#....#.#.
            #.....###..##..####.#....####.#....#..#
            
            """;

        Assert.Equal("PGUEPLPR", input.DecodePixels(AsciiFontSize._4x6));

    }
}