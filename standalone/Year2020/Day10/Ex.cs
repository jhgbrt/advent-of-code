namespace AdventOfCode.Year2020.Day10;

static class Ex
{
    internal static IEnumerable<int> FindNofConsecutiveOnes(this IEnumerable<int> differences)
    {
        var tribonnaci = new[] { 1, 1, 2, 4, 7, 13, 24 };
        int consecutiveOnes = 0;
        foreach (var d in differences)
        {
            switch (d)
            {
                case 1:
                    consecutiveOnes++;
                    break;
                case 3:
                    yield return tribonnaci[consecutiveOnes];
                    consecutiveOnes = 0;
                    break;
            }
        }
    }
}