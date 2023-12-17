using System.Collections;

namespace AdventOfCode;

public enum AsciiFontSize
{
    _4x6 = 5,
    _6x10 = 7,
}

record LetterSize(int width, int height)
{
    public int Length => width * height;
}

class AsciiFont
{
    private readonly LetterSize letterSize;
    private readonly string[] letters;
    private readonly char pixel;
    private readonly char blank;

    public AsciiFont(LetterSize letterSize, string alphabet, char pixel = '#', char blank = '.')
    {
        this.letterSize = letterSize;
        this.pixel = pixel;
        this.blank = blank;
        this.letters = (from letter in FindLetters(alphabet, 1)
                        let chars = from range in letter
                                    from c in alphabet[range]
                                    select c
                        select new string(chars.ToArray())).ToArray();
    }

    private IEnumerable<IGrouping<int, Range>> FindLetters(string s, int spacing = 1)
    => from slice in s.Lines()
       from item in slice.Chunk(letterSize.width + spacing).Select((c, i) => (c: new Range(c.Start, c.End.Value - spacing), i))
       let chunk = item.c
       let index = item.i
       group chunk by index;


    public string Encode(string s)
    {
        var range = new Range(0, letterSize.Length);
        var sb = new StringBuilder();
        foreach (var chunk in range.Chunk(letterSize.width))
        {
            foreach (var flattenedLetter in s.Select(c => letters[c - 'A']))
            {
                foreach (var c in flattenedLetter[chunk])
                {
                    sb.Append(c switch { '#' => pixel, '.' => blank, _ => throw null! });
                }
                sb.Append(blank);
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public string Decode(string s, int spacing = 1) => (
           from letter in FindLetters(s, spacing)
           let chars = from range in letter
                       from c in s[range]
                       select c == blank ? '.' : '#'
           select (
            from item in letters.Select((s,i) => (s,c: (char?)(i + 'A')))
            where item.s.SequenceEqual(chars)
            select item.c).SingleOrDefault() ?? '?'
       ).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString();


}

static class AsciiFonts
{
    public static AsciiFont GetFont(AsciiFontSize size, char pixel = '#', char blank = '.') => size switch
    {
        AsciiFontSize._6x10 => new AsciiFont(new LetterSize(6, 10), Alphabet6x10, pixel, blank),
        AsciiFontSize._4x6 => new AsciiFont(new LetterSize(4, 6), Alphabet4x6, pixel, blank),
    };

    const string Alphabet6x10 = """
        ..##...#####...####..#####..######.######..#####.#....#.#####.....###.#....#.#......#....#.#....#..####..#####...####..#####...####..######.#....#.#....#.#....#.#....#.#....#.######.
        .#..#..#....#.#....#.#....#.#......#......#......#....#...#........#..#...#..#......##..##.#....#.#....#.#....#.#....#.#....#.#........#....#....#.#....#.#....#.#....#.#....#......#.
        #....#.#....#.#......#....#.#......#......#......#....#...#........#..#..#...#......#.##.#.##...#.#....#.#....#.#....#.#....#.#........#....#....#.#....#.#....#..#..#...#..#.......#.
        #....#.#....#.#......#....#.#......#......#......#....#...#........#..#.#....#......#....#.##...#.#....#.#....#.#....#.#....#.#........#....#....#.#....#.#....#..#..#...#..#......#..
        #....#.#####..#......#....#.#####..#####..#......######...#........#..##.....#......#....#.#.#..#.#....#.#####..#....#.#####...####....#....#....#..#..#..#....#...##.....##......#...
        ######.#....#.#......#....#.#......#......#..###.#....#...#........#..##.....#......#....#.#..#.#.#....#.#......#....#.#..#........#...#....#....#..#..#..#....#...##......#.....#....
        #....#.#....#.#......#....#.#......#......#....#.#....#...#........#..#.#....#......#....#.#...##.#....#.#......#....#.#...#.......#...#....#....#..#..#..#....#..#..#.....#....#.....
        #....#.#....#.#......#....#.#......#......#....#.#....#...#....#...#..#..#...#......#....#.#...##.#....#.#......#....#.#...#.......#...#....#....#..#..#..#.##.#..#..#.....#...#......
        #....#.#....#.#....#.#....#.#......#......#....#.#....#...#....#...#..#...#..#......#....#.#....#.#....#.#......#...#..#....#......#...#....#....#...##...##..##.#....#....#...#......
        #....#.#####...####..#####..######.#.......####..#....#.#####...###...#....#.######.#....#.#....#..####..#.......###.#.#....#.#####....#.....####....##...#....#.#....#...#....######.
        """;
    const string Alphabet4x6 = """
        .##..###...##..###..####.####..##..#..#.###....##.#..#.#....#..#.#..#..##..###...##..###...###.####.#..#.#..#.#..#.#..#.#..#.####.
        #..#.#..#.#..#.#..#.#....#....#..#.#..#..#......#.#.#..#....####.##.#.#..#.#..#.#..#.#..#.#.....#...#..#.#..#.#..#.#..#.#..#....#.
        #..#.###..#....#..#.###..###..#....####..#......#.##...#....#..#.##.#.#..#.#..#.#..#.#..#.#.....#...#..#.#..#.#..#..##...##....#..
        ####.#..#.#....#..#.#....#....#.##.#..#..#......#.##...#....#..#.#.##.#..#.###..#..#.###...##...#...#..#..##..#..#.#..#...#...#...
        #..#.#..#.#..#.#..#.#....#....#..#.#..#..#...#..#.#.#..#....#..#.#.##.#..#.#....#.##.#.#.....#..#...#..#..##..####.#..#...#..#....
        #..#.###...##..###..####.#.....###.#..#.###...##..#..#.####.#..#.#..#..##..#.....###.#..#.###...#....##...##..#..#.#..#..#...####.
        """;

   
   
    public static string DecodePixels(this string s, AsciiFontSize size, char pixel = '#', char blank = '.', int spacing = 1)
    {
        var decoder = size switch
        {
            AsciiFontSize._6x10 => new AsciiFont(new LetterSize(6, 10), Alphabet6x10, pixel, blank),
            AsciiFontSize._4x6 => new AsciiFont(new LetterSize(4, 6), Alphabet4x6),
        };

        return decoder.Decode(s, spacing);
    }
}
