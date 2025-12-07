namespace AdventOfCode.Year2025.Day07;

public class AoC202507(string[] input)
{
    public AoC202507() : this(Read.InputLines()) { }

    public int Part1()
    {
        var start = input[0].IndexOf('S');
        HashSet<int> beams = [start];
        HashSet<int> newBeams = [];
        int count = 0;
        for (var row = 1; row < input.Length; row++)
        {
            newBeams.Clear();
            foreach (var x in beams)
            {
                if (input[row][x] == '^')
                {
                    newBeams.Add(x - 1);
                    newBeams.Add(x + 1);
                    count++;
                }
                else
                {
                    newBeams.Add(x);
                }
            }
            (beams, newBeams) = (newBeams, beams);
        }
        return count;
    }
    public long Part2()
    {
        var start = input[0].IndexOf('S');
        Dictionary<int, long> paths = new(input[0].Length)
        {
            [start] = 1
        };
        Dictionary<int, long> newPaths = new(input[0].Length);
        for (var row = 1; row < input.Length; row++)
        {
            newPaths.Clear();
            foreach (var (x, count) in paths)
            {
                if (input[row][x] == '^')
                {
                    // split: each path branches into two
                    newPaths[x - 1] = (newPaths.TryGetValue(x - 1, out var left) ? left : 0) + count;
                    newPaths[x + 1] = (newPaths.TryGetValue(x + 1, out var right) ? right : 0) + count;
                }
                else
                {
                    newPaths[x] = (newPaths.TryGetValue(x, out var value) ? value : 0) + count;
                }
            }

            (paths, newPaths) = (newPaths, paths);
        }

        return paths.Values.Sum();
    }
}

public class AoC202507Tests
{
    private readonly AoC202507 sut;
    public AoC202507Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202507(input);
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(21, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(40, sut.Part2());
    }
}
