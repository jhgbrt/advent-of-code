namespace AdventOfCode.Year2024.Day25;

public class AoC202425
{
    public AoC202425() : this(Read.InputStream()) { }
    public AoC202425(Stream input)
    {
        ReadInput(input);
    }

    List<long> locks = [];
    List<long> keys = [];

    private void ReadInput(Stream input)
    {
        var sr = new StreamReader(input);
        var buffer = new char[6 * 7 + 1];
        while (!sr.EndOfStream)
        {
            sr.ReadBlock(buffer);
            long value = 0;
            foreach (var c in buffer)
            {
                value = c switch
                {
                    '#' => (value << 1) | 1,
                    '.' => value << 1,
                    '\n' => value
                };
            }
            (buffer[0] switch { '#' => locks, '.' => keys }).Add(value);
        }
    }


    public int Part1()
    {
        int fits = 0;
        foreach (long l in locks)
        {
            foreach (long k in keys)
            {
                if ((l & k) == 0)
                {
                    ++fits;
                }
            }
        }
        return fits;
    }

    public int Part2() => -1;
}

public class AoC202425Tests
{
    private readonly AoC202425 sut;
    public AoC202425Tests(ITestOutputHelper output)
    {
        var input = Read.SampleStream();
        sut = new AoC202425(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(3, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(-1, sut.Part2());
    }

}