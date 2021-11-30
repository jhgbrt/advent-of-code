namespace AdventOfCode.Year2018.Day01;

static class Ex
{
    public static IEnumerable<int> EndlessRepeat(this IEnumerable<int> input)
    {
        while (true) foreach (var i in input) yield return i;
    }

}