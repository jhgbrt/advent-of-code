namespace AdventOfCode.Year2018.Day14;

static class Ex
{
    public static IEnumerable<int> GetDigits(this int num)
    {
        if (num == 0) yield return 0;
        while (num > 0)
        {
            yield return num % 10;
            num = num / 10;
        }
    }
}