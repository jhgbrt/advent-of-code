namespace AdventOfCode.Year2017.Day04;

public class AoC201704
{
    public static string[] input = Read.InputLines();

    public object Part1() => input.Count(IsValidPassword1);
    public object Part2() => input.Count(IsValidPassword2);

    private static bool IsValidPassword1(string line)
    {
        var words = line.Split(' ');
        return words.Length == words.Distinct().Count();
    }
    private static bool IsValidPassword2(string line)
    {
        var words = line.Split(' ').Select(w => new string(w.OrderBy(c => c).ToArray())).ToArray();
        return words.Length == words.Distinct().Count();
    }

}