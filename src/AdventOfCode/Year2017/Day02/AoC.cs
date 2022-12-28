namespace AdventOfCode.Year2017.Day02;

public class AoC201702
{
    public static string input = Read.InputText();

    public object Part1() => CheckSum1(new StringReader(input));
    public object Part2() => CheckSum2(new StringReader(input));
    public static int CheckSum1(TextReader reader) => ReadInts(reader)
        .Select(MinAndMax)
        .Select(x => x.max - x.min)
        .Sum();

    public static int CheckSum2(TextReader reader) => ReadInts(reader)
        .Select(EvenDivision)
        .Sum();

    static (int min, int max) MinAndMax(IEnumerable<int> input)
        => input.Aggregate(
            (min: int.MaxValue, max: int.MinValue),
            (x, n) => (n < x.min ? n : x.min, n > x.max ? n : x.max)
        );

    static int EvenDivision(IEnumerable<int> input)
    {
        var ints = input.OrderByDescending(i => i).ToArray();
        return (
            from x in ints.Select((value, index) => (idx: index, value: value))
            from y in ints.Skip(x.idx + 1)
            where (decimal)x.value % y == 0
            select x.value / y
            ).FirstOrDefault();
    }

    static IEnumerable<IEnumerable<int>> ReadInts(TextReader reader)
        => ReadLines(reader).Select(line => line.Split('\t').Select(int.Parse));

    static IEnumerable<string> ReadLines(TextReader reader)
    {
        while (reader.Peek() >= 0) yield return reader.ReadLine()!;
    }
}