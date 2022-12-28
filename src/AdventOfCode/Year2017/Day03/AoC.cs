namespace AdventOfCode.Year2017.Day03;

public class AoC201703
{
    public const int input = 265149;

    public object Part1() => DistanceToOrigin(input);
    public object Part2() => SpiralValues().SkipWhile(i => i.value <= input).First().value;

    // Part 1
    public static int DistanceToOrigin(int square)
    {
        var (x, y) = ToEuclidean(square);
        return Abs(x) + Abs(y);
    }

    // Part 2
    public static IEnumerable<(int square, (int x, int y), int value)> SpiralValues()
    {
        var values = new Dictionary<(int, int), int> { [(0, 0)] = 1 };

        int[] neighbours = { -1, 0, 1 };

        int GetAndStoreValue(int x, int y) => values[(x, y)] = (
                from i in neighbours
                from j in neighbours
                where i != 0 || j != 0
                let coordinate = (x + i, y + j)
                select values.ContainsKey(coordinate) ? values[coordinate] : 0
            ).Sum();

        return new[] { (1, (0, 0), 1) }.Concat(
            from i in InfiniteSequence().Skip(1)
            let square = i + 1
            let coordinate = ToEuclidean(square)
            let value = GetAndStoreValue(coordinate.x, coordinate.y)
            select (square, coordinate, value)
            );
    }


    // given the square position, calculates the (x,y) coordinate
    // inspired by https://stackoverflow.com/a/3715915
    internal static (int x, int y) ToEuclidean(int square)
    {
        var i = square - 1;
        var j = Round(Sqrt(i));
        int F(double l) => (int)((l + j * j - i - j % 2) * 0.5 * Pow(-1, j));
        var k = Abs(j * j - i) - j;
        return (F(k), F(-k));
    }

    static IEnumerable<int> InfiniteSequence()
    {
        for (var i = 0; i < int.MaxValue; i++) yield return i;
    }
}
