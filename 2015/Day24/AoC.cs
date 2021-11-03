using System.Diagnostics;

static class AoC
{
    public static string[] ReadInput(bool test) => File.ReadAllLines(test ? "sample.txt" : "input.txt");
    public static void Part1<T>(Func<T> f) => Run(1, f);
    public static void Part2<T>(Func<T> f) => Run(2, f);
    static void Run<T>(int part, Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        Console.WriteLine($"part {part}: {result}, took {sw.Elapsed}");
    }
}