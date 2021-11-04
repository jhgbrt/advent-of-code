
using Xunit;

Console.WriteLine(await Run.Part1());
Console.WriteLine(await Run.Part2());




static class Run
{
    static string input = File.ReadAllText("input.txt");
    public static async Task<object> Part1()
    {
        var surface = await GetDimensions().SumAsync(d => d.WrappingPaperSurface);
        return surface;
    }
    public static async Task<object> Part2()
    {
        var length = await GetDimensions().SumAsync(d => d.RibbonLength);
        return length;
    }


    static async IAsyncEnumerable<Dimension> GetDimensions()
    {
        using var reader = new StreamReader("input.txt");
        while (reader.Peek() >= 0)
        {
            string line = (await reader.ReadLineAsync())!;
            var array = line.Split('x');
            var d = new Dimension(int.Parse(array[0]), int.Parse(array[1]), int.Parse(array[2]));
            yield return d;
        }
    }

    public readonly record struct Dimension(int l, int w, int h)
    {
        public int WrappingPaperSurface => 2 * l * w + 2 * w * h + 2 * l * h + SmallestSize;
        public int RibbonLength => SmallestPerimeter + l * w * h;
        public int SmallestSize => Surfaces().Min();

        public int SmallestPerimeter => Perimeters().Min();
        IEnumerable<int> Perimeters()
        {
            yield return 2 * (l + w);
            yield return 2 * (w + h);
            yield return 2 * (h + l);
        }
        IEnumerable<int> Surfaces()
        {
            yield return l * w;
            yield return w * h;
            yield return h * l;
        }
    }
}


public class Tests
{
    [Fact]
    public async Task TestPart1()
    {
        Assert.Equal(1606483, await Run.Part1());
    }
    [Fact]
    public async Task TestPart2()
    {
        Assert.Equal(3842356, await Run.Part2());
    }
}