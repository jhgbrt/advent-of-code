namespace AdventOfCode.Year2019.Day04;

static class Ex
{
    public static IEnumerable<int> AsRange(this int[] ints) => Range(ints[0], ints[1] - ints[0]);

    public static bool HasAtLeastOneGroupOfAtLeast2AdjacentSameDigits(this int[] digits)
        => digits.GroupBy(i => i).Any(g => g.Count() >= 2);
    public static bool HasAtLeastOneGroupOfExactly2AdjacentSameDigits(this int[] digits)
        => digits.GroupBy(i => i).Any(g => g.Count() == 2);


    public static int[] ToDigits(this int n)
    {
        int[] digits = new int[6];
        int t = n;
        for (int i = 5; i >= 0; i--)
        {
            digits[i] = t % 10;
            t /= 10;
        }
        return digits;
    }

    public static bool IsAscending(this int[] digits)
        => digits.Take(digits.Length - 1).Zip(digits.Skip(1)).All(x => x.First <= x.Second);

}