using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    public static string[] input = File.ReadAllLines("input.txt");

    public static Result Part1() => Run(() => null);
    public static Result Part2() => Run(() => null);

    static Result Run(Func<object> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        return new(result, sw.Elapsed);
    }
}

