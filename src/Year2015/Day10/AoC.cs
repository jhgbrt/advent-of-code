namespace AdventOfCode.Year2015.Day10;

public class AoC201510 : AoCBase
{
    static readonly string input = "1113222113";

    public override object Part1() => Run(input, 40);
    public override object Part2() => Run(input, 50);

    static int Run(string input, int times)
    {
        var sb = new StringBuilder();

        for (int i = 0; i < times; i++)
        {
            sb.Clear();
            char last = default;
            int n = 0;
            foreach (var c in input)
            {
                if (last == c)
                {
                    n++;
                }
                else
                {
                    if (last != default) sb.Append(n).Append(last);
                    last = c;
                    n = 1;
                }
            }
            sb.Append(n).Append(last);
            input = sb.ToString();
        }
        return sb.Length;
    }
}