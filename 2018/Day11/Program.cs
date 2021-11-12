using static AoC;
using AdventOfCode;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static int input = int.Parse(File.ReadAllText("input.txt"));

    internal static Result Part1() => Run(() => 
    {
        var result = Part1(input);
        return $"{result.left},{result.top}";
    });
    internal static Result Part2() => Run(() => 
    { 
        var result = Part2(input);
        return $"{result.top},{result.left},{result.s}";
    });

    public static (int left, int top, int power) Part1(int sn)
    {
        var result = (
                   from x in Enumerable.Range(1, 300)
                   from y in Enumerable.Range(1, 300)
                   let p = GetSquarePower((x, y), sn)
                   orderby p descending
                   select (x, y, p)
               ).First();
        return result;
    }

    public static (int top, int left, int s, int p) Part2(int sn)
    {
        var grid = (
            from y in Enumerable.Range(1, 300)
            from x in Enumerable.Range(1, 300)
            let p = GetCellPower(x, y, sn)
            select (x, y, p)
        ).Aggregate(
            new int[301, 301],
            (sum, t) => { sum[t.y, t.x] = t.p + sum[t.y - 1, t.x] + sum[t.y, t.x - 1] - sum[t.y - 1, t.x - 1]; return sum; });

        (int bx, int by, int bs, int best) = (
            from s in Enumerable.Range(1, 300)
            from y in Enumerable.Range(s, 300 - s + 1)
            from x in Enumerable.Range(s, 300 - s + 1)
            let p = grid[y, x] - grid[y - s, x] - grid[y, x - s] + grid[y - s, x - s]
            orderby p descending
            select (x, y, s, p)
        ).First();

        return (bx - bs + 1, by - bs + 1, bs, best);
    }

    public static int GetCellPower(int x, int y, int serialNumber)
        => ((x + 10) * y + serialNumber) * (x + 10) / 100 % 10 - 5;

    public static int GetSquarePower((int x, int y) location, int sn, int size = 3)
        => (from x in Enumerable.Range(location.x, size)
            from y in Enumerable.Range(location.y, size)
            select GetCellPower(x, y, sn)).Sum();

}