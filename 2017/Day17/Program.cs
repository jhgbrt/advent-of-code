

using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    const int input = 377;
    internal static Result Part1() => Run(() =>
    {
        var result = Spinlock.Find(input, 2017);
        return result.buffer[result.index + 1];
    });
    //internal static Result Part1() => Run(() => Spinlock.FindFast(input, 2017));
    internal static Result Part2() => Run(() => Spinlock.FindFast(input, 50_000_000));

}
