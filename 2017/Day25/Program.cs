using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string input = File.ReadAllText("input.txt");
    internal static Result Part1() => Run(() => File.ReadAllText("input.txt").EncodeToSomethingSimpler().CalculateChecksum());
    internal static Result Part2() => Run(() => null);
}

