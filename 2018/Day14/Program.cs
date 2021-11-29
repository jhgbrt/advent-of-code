using static AdventOfCode.Year2018.Day14.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day14
{
    partial class AoC
    {
        static int input = int.Parse(File.ReadLines("input.txt").First());

        internal static Result Part1() => Run(() => Part1(input));
        internal static Result Part2() => Run(() => Part2(input));

        public static long Part1(int n)
        {
            int i = 0;
            int j = 1;
            var recipes = new List<int> { 3, 7 };
            while (recipes.Count < (n + 10))
            {
                var sum = recipes[i] + recipes[j];
                recipes.AddRange(sum.GetDigits().Reverse());
                i = (i + 1 + recipes[i]) % recipes.Count;
                j = (j + 1 + recipes[j]) % recipes.Count;
            }

            return (
                from x in Enumerable.Range(0, 10)
                let p = (long)Math.Pow(10, 10 - x - 1)
                let r = recipes[n + x]
                select p * r
             ).Sum();
        }

        public static long Part2(int input)
        {
            var digits = input.GetDigits().Reverse().ToArray();
            while (digits.Length < 5) digits = new[] { 0 }.Concat(digits).ToArray();

            int index = 0;
            int offset = 0;
            bool found = false;
            int i = 0;
            int j = 1;
            var recipes = new List<int> { 3, 7 };
            while (!found)
            {
                int sum = recipes[i] + recipes[j];
                recipes.AddRange(sum.GetDigits().Reverse());

                i = (i + 1 + recipes[i]) % recipes.Count;
                j = (j + 1 + recipes[j]) % recipes.Count;

                if (recipes.Count < digits.Length) continue;

                while (!found && index + offset < recipes.Count)
                {
                    if (digits[offset] == recipes[index + offset])
                    {
                        if (offset < digits.Length - 1)
                            offset++;
                        else
                            found = true;
                    }
                    else
                    {
                        offset = 0;
                        index++;
                    }
                }
            }
            return index;
        }
    }
}

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

