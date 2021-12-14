namespace AdventOfCode.Year2019.Day09;

public class AoC201909
{
    internal static string[] input = Read.InputLines(typeof(AoC201909));

    public object Part1()
    {
        foreach (var i in IntCode.Run(input.First().Split(',').Select(long.Parse).Select((n, i) => (n, i: (long)i)).ToImmutableDictionary(x => x.i, x => x.n), 1))
        {
            //Console.WriteLine(i);
        }
        return -1;
    }

    public object Part2() => -1;

    [Fact]
    public void ShouldCopyItSelf()
    {
        var program = new[] { 109L, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99 }.Select((value,index) => (value,index:(long)index)).ToImmutableDictionary(x=>x.index, x => x.value);
        var result = IntCode.Run(program);
        Assert.Equal(program.Values, result);
    }

    [Fact]
    public void ShouldHave16Digits()
    {
        var program = new[] { 1102L, 34915192, 34915192, 7, 4, 7, 99, 0 }.Select((value, index) => (value, index: (long)index)).ToImmutableDictionary(x => x.index, x => x.value);
        var result = IntCode.Run(program);
        Assert.Equal(16, result.First().ToString().Length);
    }
    [Fact]
    public void ShouldOutputLargeInput()
    {
        var program = new[] { 104, 1125899906842624, 99 }.Select((value, index) => (value, index: (long)index)).ToImmutableDictionary(x => x.index, x => x.value);
        var result = IntCode.Run(program);
        Assert.Equal(1125899906842624, result.First());
    }

}