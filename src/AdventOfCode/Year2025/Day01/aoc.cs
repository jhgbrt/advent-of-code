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
        var next = (acc.value + 100 + i) % 100;
        var increment = i >= 0
            ? (acc.value + i) / 100
            : (-i - acc.value + (acc.value == 0 ? 0 : 100)) / 100;
        return acc with { value = next, password = acc.password + increment };
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

}