namespace AdventOfCode.Year2017.Day08;

public class Tests
{
    // if (?<testregister>) (?<comparison>[<>=!]{1,2}) (?<comparisonValue>)
    [Theory]
    [InlineData("b inc 5 if a > 1")]
    public void TestRegex(string input)
    {
        var result = Instruction.r.Match(input);
        Assert.True(result.Success);
        Assert.Equal("b", result.Groups["register"].Value);
        Assert.Equal("inc", result.Groups["operation"].Value);
        Assert.Equal("5", result.Groups["amount"].Value);
        Assert.Equal("a", result.Groups["testregister"].Value);
        Assert.Equal(">", result.Groups["operator"].Value);
        Assert.Equal("1", result.Groups["comparisonValue"].Value);
    }

    [Fact]
    public void TestInstructionParse()
    {
        var input = "abc dec -25 if def > -5";
        var instruction = Instruction.Parse(input);

        Assert.Equal("abc", instruction.Register);

        var dictionary = new Dictionary<string, int> { ["abc"] = 0, ["def"] = -5 };

        instruction.Apply(dictionary);
        Assert.Equal(0, dictionary["abc"]);

        dictionary["def"] = -4;
        instruction.Apply(dictionary);
        Assert.Equal(25, dictionary["abc"]);
    }


    [Fact]
    public void Test()
    {

        var sample = @"b inc 5 if a > 1
a inc 1 if b < 5
c dec -10 if a >= 1
c inc -20 if c == 10";

        var instructions = ReadLines(sample).Select(Instruction.Parse).ToImmutableList();
        var cpu = new Cpu(instructions).Run();

        Assert.Equal(1, cpu.MaxCurrentValue());

    }

    private IEnumerable<string> ReadLines(string s)
    {
        using (var reader = new StringReader(s))
        {
            while (reader.Peek() >= 0)
                yield return reader.ReadLine();
        }
    }
}
