namespace AdventOfCode.Year2015.Day01;
public class AoC201501
{
    static readonly string input = Read.InputText(typeof(AoC201501));

    public object Part1() => input.Select(c => c switch { '(' => +1, ')' => -1, _ => throw new Exception() }).Sum();
    public object Part2()
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
