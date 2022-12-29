namespace AdventOfCode;

public enum PixelFontSize
{
    _4x6 = 5,
    _6x10 = 7,
}

static class AsciiFontDecoder
{

    static readonly string[] _4x6 = new[]
        {
            ".##.#..##..######..##..#",
            "###.#..####.#..##..####.",
            ".##.#..##...#...#..#.##.",
            "###.#..##..##..##..####.",
            "#####...###.#...#...####",
            "#####...###.#...#...#...",
            ".##.#..##...#.###..#.###",
            "#..##..######..##..##..#",
            "###..#...#...#...#..###.",
            "..##...#...#...##..#.##.",
            "#..##.#.##..##..#.#.#..#",
            "#...#...#...#...#...####",
            "#..######..##..##..##..#",
            "#..###.###.##.###.###..#",
            ".##.#..##..##..##..#.##.",
            "###.#..##..####.#...#...",
            ".##.#..##..##..##.##.###",
            "###.#..##..####.#.#.#..#",
            ".####...#....##....####.",
            "####.#...#...#...#...#..",
            "#..##..##..##..##..#.##.",
            "#..##..##..#.##..##..##.",
            "#..##..##..##..######..#",
            "#..##..#.##.#..##..##..#",
            "#..##..#.##...#...#..#..",
            "####...#..#..#..#...####"
        };

    static readonly string[] _6x10 = new[]
    {
        "..##...#..#.#....##....##....########....##....##....##....#",
        "#####.#....##....##....######.#....##....##....##....######.",
        ".####.#....##.....#.....#.....#.....#.....#.....#....#.####.",
        "#####.#....##....##....##....##....##....##....##....######.",
        "#######.....#.....#.....#####.#.....#.....#.....#.....######",
        "#######.....#.....#.....#####.#.....#.....#.....#.....#.....",
        ".######.....#.....#.....#.....#..####....##....##....#.####.",
        "#....##....##....##....########....##....##....##....##....#",
        "#####...#.....#.....#.....#.....#.....#.....#.....#...#####.",
        "...###....#.....#.....#.....#.....#.....#.#...#.#...#..###..",
        "#....##...#.#..#..#.#...##....##....#.#...#..#..#...#.#....#",
        "#.....#.....#.....#.....#.....#.....#.....#.....#.....######",
        "#....###..###.##.##....##....##....##....##....##....##....#",
        "#....##....###...###...##.#..##..#.##...###...###....##....#",
        ".####.#....##....##....##....##....##....##....##....#.####.",
        "#####.#....##....##....######.#.....#.....#.....#.....#.....",
        ".####.#....##....##....##....##....##....##....##...#..###.#",
        "#####.#....##....##....######.#..#..#...#.#...#.#....##....#",
        ".####.#.....#.....#......####......#.....#.....#.....######.",
        "######..#.....#.....#.....#.....#.....#.....#.....#.....#...",
        "#....##....##....##....##....##....##....##....##....#.####.",
        "#....##....##....##....#.#..#..#..#..#..#..#..#...##....##..",
        "#....##....##....##....##....##....##....##.##.###..###....#",
        "#....##....#.#..#..#..#...##....##...#..#..#..#.#....##....#",
        "#....##....#.#..#..#..#...##.....#.....#.....#.....#....#...",
        "######.....#.....#....#....#....#....#....#.....#.....######"

    };
    static IReadOnlyDictionary<PixelFontSize, (string s, char c)[]> lettersBySize = new[]
    {
        (size: PixelFontSize._4x6, letters: _4x6.Select((s, i) => (s, (char)(i + 'A'))).ToArray()),
        (size: PixelFontSize._6x10, letters: _6x10.Select((s, i) => (s, (char)(i + 'A'))).ToArray())
    }.ToDictionary(x => x.size, x => x.letters);

    // use this function to generate a simple string representing the character
    public static IEnumerable<(string, char)> FlattenLetters(string s, PixelFontSize size, char pixel = '#', char blank = '.')
        => from letter in FindLetters(s, size)
           let flattenedValue = letter
            .Aggregate(new StringBuilder(), (sb, range) => sb.Append(s[range]).Replace(pixel, '#').Replace(blank, '.')).ToString()
           let result = lettersBySize[size].Where(l => l.s == flattenedValue).Select(l => (char?)l.c).FirstOrDefault() ?? '?'
           select (flattenedValue, result);

    public static string DecodePixels(this string s, PixelFontSize size, char pixel = '#', char blank = '.') => (
            from letter in FindLetters(s, size)
            let chars = from range in letter from c in s[range] select c switch { '#' => pixel, _ => blank }
            select (from item in lettersBySize[size] where item.s.SequenceEqual(chars) select (char?)item.c).SingleOrDefault() ?? '?'
        ).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString();

    private static IEnumerable<IGrouping<int, Range>> FindLetters(string s, PixelFontSize size)
        => from slice in s.Lines()
           from item in slice.Chunk(size switch
           {
               PixelFontSize._4x6 => 5,
               PixelFontSize._6x10 => 7,
               _ => throw new NotImplementedException($"Cannot determine size for {size}")
           }).Select((c, i) => (c: new Range(c.Start, c.End.Value - 1), i))
           let chunk = item.c
           let index = item.i
           group chunk by index;

    public static string Encode(this string s, PixelFontSize size)
    {
        var q = from c in s
                let flattenedLetter = lettersBySize[size].Where(item => item.c == c).Single()
                select flattenedLetter.s;

        var sb = new StringBuilder();

        var range = new Range(0, lettersBySize[size][0].s.Length);
        foreach (var chunk in range.Chunk(size switch 
        { 
            PixelFontSize._4x6 => 4, 
            PixelFontSize._6x10 => 6, 
            _ => throw new NotImplementedException($"Cannot determine size for {size}")
        }))
        {
            foreach (var f in q)
            {
                sb.Append(f[chunk]).Append('.');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    private static IEnumerable<Range> Lines(this string s)
    {
        int x = 0;
        while (x < s.Length)
        {
            var newline = s.IndexOf('\n', x);
            if (newline == -1)
            {
                if (!s.EndsWith('\n'))
                {
                    yield return new(x, s.Length);
                }
                break;
            }
            var count = newline switch { > 0 when s[newline - 1] == '\r' => newline - x - 1, _ => newline - x };
            yield return new(x, x + count);
            x = newline + 1;
        }
    }
    private static IEnumerable<Range> Chunk(this Range range, int size)
    {
        int s = range.Start.Value;
        while (s < range.End.Value)
        {
            yield return new Range(s, s + (size > range.End.Value ? range.End.Value - s : size));
            s += size;
        }
    }

}
