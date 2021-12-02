namespace AdventOfCode;

record Result(object Value, TimeSpan Elapsed);
record Answer(object? part1, object? part2)
{
    public static Answer Empty = new Answer(null, null);
}

public abstract class AoCBase
{
    readonly Answer answer;
    int Year => int.Parse(GetType().Namespace.AsSpan().Slice(17, 4));
    int Day => int.Parse(GetType().Namespace.AsSpan().Slice(25, 2));
    public AoCBase()
    {
        answer = Read.Answers(GetType());
    }

    [SkippableFact]
    public void TestPart1()
    {
        Skip.If(answer.part1 is null, $"Puzzle 1 for {Year}/{Day} has not yet been answered");
        var result = Part1();
        Skip.If(result is -1, $"Puzzle 1 for for {Year}/{Day} has an answer but no code");
        Assert.Equal(answer.part1.ToString(), result.ToString());
    }

    [SkippableFact]
    public void TestPart2()
    {
        if (Day == 25) return;
        Skip.If(answer.part2 is null, $"Puzzle 2 for {Year}/{Day} has not yet been answered");
        var result = Part2();
        Skip.If(result is -1, $"Puzzle 2 for for {Year}/{Day} has an answer but no code");
        Assert.Equal(answer.part2.ToString(), result.ToString());
    }

    public abstract object Part1();
    public abstract object Part2();
}
