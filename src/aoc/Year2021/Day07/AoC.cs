namespace AdventOfCode.Year2021.Day07;

public class AoC202107
{
    static string input = Read.InputText();
    static ImmutableArray<int> numbers = input.Split(',').Select(int.Parse).ToImmutableArray();
    public object Part1() => (from i in Range(0, numbers.Max())
                                       let cost = (from j in numbers select Cost1(i,j)).Sum()
                                       select cost).Min();

    private static int Cost1(int x, int y) => Math.Abs(y - x);
    private static int Cost2(int x, int y)
    {
        var distance = Cost1(x, y);
        return distance * (distance + 1) / 2;
    }

    public object Part2() => (from i in Range(0, numbers.Max())
                                       let cost = (from j in numbers select Cost2(i, j)).Sum()
                                       select cost).Min();
}
