namespace AdventOfCode.Year2015.Day01;
public class AoCImpl : AoCBase
{
    static readonly string input = Read.InputText(typeof(AoCImpl));

    public override object Part1() => input.Select(c => c switch { '(' => +1, ')' => -1, _ => throw new Exception() }).Sum();
    public override object Part2()
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
