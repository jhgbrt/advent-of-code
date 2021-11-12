using System.Text;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static readonly string input = "1113222113";

    internal static Result Part1() => Run(input, 40);
    internal static Result Part2() => Run(input, 50);

    static Result Run(string input, int times)
    {
        var sw = Stopwatch.StartNew();
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
        return new(sb.Length, sw.Elapsed);
    }
}public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(252594, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(3579328, Part2().Value);
}
