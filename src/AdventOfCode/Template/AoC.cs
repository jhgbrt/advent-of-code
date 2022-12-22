namespace AdventOfCode.YearYYYY.DayDD;
public class AoCYYYYDD
{
    static bool usesample = true;
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();
    static string[] theinput = usesample ? sample : input;
    public object Part1()
    {
        Console.WriteLine(string.Join(Environment.NewLine, theinput));
        return -1;
    }
    public object Part2() => "";
}
