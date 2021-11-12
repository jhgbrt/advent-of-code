
using dance;


using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static Dancer? dancer = new Dancer(new StreamReader("input.txt"));
    static string initial = "abcdefghijklmnop";
    static int seedA = 722;    
    internal static Result Part1() => Run(() => dancer.Run(initial));
    internal static Result Part2() => Run(() => dancer.Run(initial, 1000000000));

}

