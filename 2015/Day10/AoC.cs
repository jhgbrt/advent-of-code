namespace AdventOfCode.Year2015.Day10;

record Result(object Value, TimeSpan Elapsed);
record Answer(object? part1, object? part2);

public partial class AoC
{
    ITestOutputHelper output;

    public AoC(ITestOutputHelper output)
    {
        this.output = output;
    }

    Answer answer = JsonSerializer.Deserialize<Answer>(File.ReadAllText("answers.json"))!;
    [Fact]
    public void TestPart1()
    {
        if (answer.part1 is not null)
        {
            var result = AoC.Part1().Value;
            if (result is null) throw new Exception("Puzzle 1 has an answer but no code");
            Assert.Equal(answer.part1.ToString(), AoC.Part1().Value.ToString());
        }
        else
        {
            throw new Exception("Puzzle 1 has not yet been answered");
        }
    }

    [Fact]
    public void TestPart2()
    {
        if (System.Reflection.Assembly.GetExecutingAssembly().GetName().Name!.EndsWith("25")) return;
        if (answer.part2 is not null)
        {
            var result = AoC.Part1().Value;
            if (result is null) throw new Exception("Puzzle 1 has an answer but no code");
            Assert.Equal(answer.part2.ToString(), AoC.Part2().Value.ToString());
        }
        else
        {
            throw new Exception("Puzzle 2 has not yet been answered");
        }
    }

    internal static Result Run(Func<object> f)
    {
        var sw = Stopwatch.StartNew();
        return new(f(), sw.Elapsed);
    }
}
