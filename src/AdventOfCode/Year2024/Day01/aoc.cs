namespace AdventOfCode.Year2024.Day01;
public class AoC202401(Stream stream, int size)
{
    public AoC202401() : this(Read.InputStream(), 5) {}

    readonly (List<int> left, List<int> right) input = ReadInput(stream, size);
    static (List<int> left, List<int> right) ReadInput(Stream stream, int size)
    {
        var l = new List<int>(1000);
        var r = new List<int>(1000);

        var sr = new StreamReader(stream);
        Span<char> buffer = stackalloc char[2*size + 3];

        // file contains lines of 5 digits, 3 spaces, 5 digits and \n (= 14 characters)
        // the first 5 digits are the left number, the second 5 digits are the right number
        while (!sr.EndOfStream)
        {
            sr.ReadBlock(buffer);
            l.Add(int.Parse(buffer[0..size]));
            r.Add(int.Parse(buffer[(size + 3)..(2*size +3)]));
            while (sr.Peek() is '\r' or '\n') sr.Read();
        }
        return (l, r);
    }

    public int Part1()
    {
        var (left, right) = input;
        return left.Order().Zip(right.Order(), (l, r) => Abs(r - l)).Sum();
    }

    public long Part2()
    {
        var (left, right) = input;

        var counts = (
            from r in right 
            group r by r into g 
            select (g.Key, Count: g.Count())
         ).ToDictionary(x => x.Key);

        return (from l in left
                select counts.ContainsKey(l) ? l * counts[l].Count : 0).Sum();

    }
}


public class AoC202401Tests
{
    private readonly AoC202401 sut;
    public AoC202401Tests(ITestOutputHelper output)
    {
        var input = Read.SampleStream();
        sut = new AoC202401(input, 1);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(11, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(31, sut.Part2());
    }
}