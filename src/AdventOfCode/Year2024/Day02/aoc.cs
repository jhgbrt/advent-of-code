namespace AdventOfCode.Year2024.Day02;
public class AoC202402(Stream stream)
{
    public AoC202402() : this(Read.InputStream()) {}

    readonly IReadOnlyList<int>[] input = ReadInput(stream).ToArray();

    private static IEnumerable<IReadOnlyList<int>> ReadInput(Stream stream)
    {
        using var r = new StreamReader(stream);
        while (!r.EndOfStream)
        {
            var span = r.ReadLine().AsSpan();
            List<int> ints = [];
            foreach (var range in span.Split(' '))
            {
                ints.Add(int.Parse(span[range]));
            }
            yield return ints;
        }
    }

    public int Part1() => input.Where(IsSafe).Count();
    public int Part2() => input.Where(IsSafe2).Count();

    public static bool IsSafe(IReadOnlyList<int> list)
    {
        var ascending = list[1] > list[0];
        for (int i = 0; i < list.Count - 1; i++)
        {
            var delta = list[i + 1] - list[i];
            if (delta is 0) return false;
            if (delta > 0 != ascending) return false;
            if (Abs(delta) is < 1 or > 3) return false;
        }

        return true;
    }
    public static bool IsSafe2(IReadOnlyList<int> list)
    {
        var buffer = new List<int>(list.Count - 1);
        for (var exclude = 0; exclude < list.Count; exclude++)
        {
            buffer.AddRange(Except(list, exclude));
            if (IsSafe(buffer)) return true;
            buffer.Clear();
        }
        return false;
    }
    static IEnumerable<int> Except(IReadOnlyList<int> list, int item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (i == item) continue;
            yield return list[i];
        }
    }
}

public class AoC202402Tests
{
    private readonly AoC202402 sut;
    public AoC202402Tests(ITestOutputHelper output)
    {
        var input = Read.SampleStream();
        sut = new AoC202402(input);
    }


    [Theory]
    [InlineData(false, 16, 10, 15, 5, 1, 11, 7, 19, 6, 12, 4)]
    [InlineData(false, 16, 17, 19, 20, 18)]
    public void IsSafeTest(bool expected, params int[] args)
    {
        Assert.Equal(expected, AoC202402.IsSafe(args.ToList()));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(2, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(4, sut.Part2());
    }
}