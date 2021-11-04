using Xunit;

Console.WriteLine(Run.Part1());
Console.WriteLine(Run.Part2());

static class Run
{
    static string input = File.ReadAllText("input.txt");
    public static object Part1()
    {
        var result1 = input.Select(c => c switch { '(' => +1, ')' => -1, _ => throw new Exception() }).Sum();
        return result1;
    }
    public static object Part2()
    {
        var sum = 0;
        for (int i = 0; i < input.Length; i++)
        {
            sum += input[i] switch { '(' => +1, ')' => -1, _ => throw new Exception() };
            if (sum == -1) return i + 1;
        }
        return -1;
    }
}

public class Tests
{
    [Fact]
    public void TestPart1()
    {
        Assert.Equal(232, Run.Part1());
    }
    [Fact]
    public void TestPart2()
    {
        Assert.Equal(1783, Run.Part2());
    }
}