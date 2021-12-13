namespace AdventOfCode.Year2021.Day13;

public class AoC202113 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC202113));
    static Regex coordinateRegex = new Regex(@"^(?<x>\d+),(?<y>\d+)$");
    static Regex foldRegex = new Regex(@"^fold along (?<coordinate>x|y)=(?<value>\d+)$");
    static Grid grid = new Grid(
        from line in input
        let match = coordinateRegex.Match(line)
        where match.Success
        select new Coordinate(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value))
    );
    static IEnumerable<(char, int)> folding = from line in input let match = foldRegex.Match(line) where match.Success select (match.Groups["coordinate"].Value[0], int.Parse(match.Groups["value"].Value));

    public override object Part1() => FoldingCycle(grid, folding).First().Count();
    public override object Part2() => FoldingCycle(grid, folding).Last().ToString().DecodeAscii(5);

    static IEnumerable<Grid> FoldingCycle(Grid grid, IEnumerable<(char, int)> instructions)
    {
        foreach (var (c, value) in instructions)
        {
            grid = grid.Fold(c, value);
            yield return grid;
        }
    }
}


class Grid
{
    ImmutableHashSet<Coordinate> coordinates;
    int MaxX;
    int MaxY;
    public Grid(IEnumerable<Coordinate> coordinates)
    {
        this.coordinates = coordinates.ToImmutableHashSet();
        MaxX = coordinates.MaxBy(c => c.x)!.x;
        MaxY = coordinates.MaxBy(c => c.y)!.y;
    }

    public override string ToString()
    {
        
        var sb = new StringBuilder();
        for (var y = 0; y <= MaxY; y++)
        {
            for (var x = 0; x <= MaxX; x++)
            {
                sb.Append(coordinates.Contains(new(x, y)) ? '#' : '.');
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }

    public Grid Fold(char c, int v) => new Grid(c switch
    {
        'y' => FoldUp(coordinates, v),
        'x' => FoldLeft(coordinates, v),
        _ => throw new Exception()
    });

    private ImmutableHashSet<Coordinate> FoldUp(ImmutableHashSet<Coordinate> coordinates, int v)
        => Fold(coordinates, from d in Range(1, MaxY - v + 1)
                             from x in Range(0, MaxX + 1)
                             where coordinates.Contains(new(x, d + v))
                             select (src: new Coordinate(x, v + d), to: new Coordinate(x, v - d)));
    private ImmutableHashSet<Coordinate> FoldLeft(ImmutableHashSet<Coordinate> coordinates, int v)
        => Fold(coordinates, from d in Range(1, MaxX - v + 1)
                             from y in Range(0, MaxY + 1)
                             where coordinates.Contains(new(d + v, y))
                             select (src: new Coordinate(v + d, y), to: new Coordinate(v - d, y)));
    private ImmutableHashSet<Coordinate> Fold(ImmutableHashSet<Coordinate> coordinates, IEnumerable<(Coordinate src, Coordinate to)> transformations)
    {
        foreach (var c in transformations)
            coordinates = coordinates.Remove(c.src).Add(c.to);
        return coordinates;
    }

    internal int Count() => coordinates.Count;
}

record Coordinate(int x, int y);

static class PixelFontDecoder
{
    static readonly string[] _4x6 = new[]
        {
            ".##.#..##..######..##..#",
            "###.#..####.#..##..####.",
            ".####...#...#...#....###",
            "###.#..##..##..##..####.",
            "#####...###.#...#...####",
            "#####...###.#...#...#...",
            ".####...#...#.###..#.##.",
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
        "#....##....##....##....#.#..#..#..#..#. #..#..#...##....##..",
        "#....##....##....##....##....##....##....##.##.###..###....#",
        "#....##....#.#..#..#..#...##....##...#..#..#..#.#....##....#",
        "#....##....#.#..#..#..#...##.....#.....#.....#.....#....#...",
        "######.....#.....#....#....#....#....#....#.....#.....######"

    };
    static IReadOnlyDictionary<int, (string s, char c)[]> lettersBySize = new[]
    {
        (size: 5, letters: _4x6.Select((s, i) => (s, (char)(i + 'A'))).ToArray()),
        (size: 7, letters: _6x10.Select((s, i) => (s, (char)(i + 'A'))).ToArray())
    }.ToDictionary(x => x.size, x => x.letters);

    // use this function to generate a simple string representing the character
    public static IEnumerable<(string, char)> FlattenLetters(string s, int size, char pixel = '#', char blank = '.')
        => from letter in FindLetters(s, size)
           let flattenedValue = letter.Aggregate(new StringBuilder(), (sb, range) => sb.Append(s[range]).Replace(pixel, '#').Replace(blank, '.')).ToString()
           let result = lettersBySize[size].Where(l => l.s == flattenedValue).Select(l => (char?)l.c).FirstOrDefault() ?? '?'
           select (flattenedValue, result);

    public static string DecodeAscii(this string s, int size, char pixel = '#', char blank = '.') => (
            from letter in FindLetters(s, size)
            let chars = from range in letter from c in s[range] select c switch { '#' => pixel, _ => blank }
            select (from item in lettersBySize[size] where item.s.SequenceEqual(chars) select (char?)item.c).SingleOrDefault() ?? '?'
        ).Aggregate(new StringBuilder(), (sb, c) => sb.Append(c)).ToString();

    private static IEnumerable<IGrouping<int, Range>> FindLetters(string s, int size)
        => from slice in s.Lines()
           from item in slice.Chunk(size).Select((c, i) => (c: new Range(c.Start, c.End.Value - 1), i))
           let chunk = item.c
           let index = item.i
           group chunk by index;

    private static IEnumerable<Range> Lines(this string s)
    {
        int x = 0;
        while (x < s.Length)
        {
            var newline = s.IndexOf('\n', x);
            if (newline == -1) break;
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

