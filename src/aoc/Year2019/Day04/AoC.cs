using static System.Linq.Enumerable;

namespace AdventOfCode.Year2019.Day04;

public class AoC201904
{
    static string[] input = Read.InputLines();

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);
    public static int Part1(string[] input)
        => (
        from i in ParseInput(input)
        let d = i.ToDigits()
        where d.IsAscending() && d.HasAtLeastOneGroupOfAtLeast2AdjacentSameDigits()
        select d
        ).Count();

    public static int Part2(string[] input)
        => (
        from i in ParseInput(input)
        let d = i.ToDigits()
        where d.IsAscending() && d.HasAtLeastOneGroupOfExactly2AdjacentSameDigits()
        select d
        ).Count();

    static IEnumerable<int> ParseInput(string[] input)
        => input[0]
        .Split('-')
        .Select(int.Parse)
        .ToArray()
        .AsRange();
}