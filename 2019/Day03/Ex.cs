namespace AdventOfCode.Year2019.Day03;

static class Ex
{
    public static IEnumerable<(int x, int y)> Points(this string input)
    {
        (int x, int y) = (0, 0);
        foreach (var item in input.Split(','))
        {
            Func<int, int, (int, int)> f = item[0] switch
            {
                'R' => (x, y) => (x + 1, y),
                'L' => (x, y) => (x - 1, y),
                'U' => (x, y) => (x, y + 1),
                'D' => (x, y) => (x, y - 1),
                _ => throw new Exception()
            };
            var d = int.Parse(item.AsSpan().Slice(1));
            for (int i = 0; i < d; i++)
            {
                (x, y) = f(x, y);
                yield return (x, y);
            }
        }
    }
}