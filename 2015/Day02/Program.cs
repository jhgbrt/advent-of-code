
var surface = await GetDimensions().SumAsync(d => d.WrappingPaperSurface);
Console.WriteLine(surface);
var length = await GetDimensions().SumAsync(d => d.RibbonLength);
Console.WriteLine(length);

static async IAsyncEnumerable<Dimension> GetDimensions()
{
    using var reader = new StreamReader("input.txt");
    while (reader.Peek() >= 0)
    {
        var line = await reader.ReadLineAsync();
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
        yield return l*w;
        yield return w*h;
        yield return h*l;
    }
}
