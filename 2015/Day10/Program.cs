using System.Text;

using Xunit;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

static class AoC
{
    static readonly string input = "1113222113";

    public static object Part1() => Run(input, 40);
    public static object Part2() => Run(input, 50);

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
}public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(252594, Part1());
    [Fact]
    public void Test2() => Assert.Equal(3579328, Part2());
}
