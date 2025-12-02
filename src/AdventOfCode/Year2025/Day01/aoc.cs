namespace AdventOfCode.Year2025.Day01;

public class AoC202501
{
    int[] instructions;
    public AoC202501(string[] input)
    {
        instructions = [.. input.Select(line => (line[0] == 'L' ? -1 : 1) * int.Parse(line[1..]))];
    }

    public AoC202501() : this(Read.InputLines()) 
    {
    }

    public int Part1() => instructions.Aggregate((value: 50, password: 0), (acc, i) =>
    {
        var next = (acc.value + 100 + i) % 100;
        var increment = next == 0 ? 1 : 0;
        return acc with { value = next, password = acc.password + increment };
    }).password;

    public int Part2() => instructions.Aggregate((value: 50, password: 0), (acc, i) =>
    {
        int dir = Math.Sign(i);
        int steps = Math.Abs(i);
        int count = 0;
        for (int k = 1; k <= steps; k++)
        {
            int next = ((acc.value + k * dir) % 100 + 100) % 100;
            if (next == 0) count++;
        }
        int final = ((acc.value + steps * dir) % 100 + 100) % 100;
        return (value: final, password: acc.password + count);
    }).password;
}

public class AoC202501Tests
{
    private readonly AoC202501 sut;
    public AoC202501Tests()
    {
        var input = Read.SampleLines();
        sut = new AoC202501(input);
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
        Assert.Equal(6, sut.Part2());
    }

    [Fact]
    public void Part2RealInput()
    {
        var sut = new AoC202501(Read.InputLines());
        Assert.Equal(6616, sut.Part2());
    }

}