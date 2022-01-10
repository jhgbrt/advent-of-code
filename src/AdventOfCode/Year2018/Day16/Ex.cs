namespace AdventOfCode.Year2018.Day16;

static class Ex
{
    internal static IEnumerable<(int code, int a, int b, int c)> GetInstructions(this string[] input)
    {
        int blank = 0;
        int i = 0;
        for (i = 0; i < input.Length; i++)
        {
            if (string.IsNullOrEmpty(input[i])) blank++;
            else blank = 0;
            if (blank == 3)
                break;
        }
        for (i = i + 1; i < input.Length; i++)
        {
            var instruction = input[i].Split(' ').Select(int.Parse).ToArray();
            yield return (instruction[0], instruction[1], instruction[2], instruction[3]);
        }
    }

    internal static IEnumerable<(int[] before, (int code, int a, int b, int c) instruction, int[] after)> Parse(this string[] input)
    {
        for (int i = 0; i < input.Length; i += 4)
        {
            if (input[i].StartsWith("Before"))
            {
                var before = input[i][9..^1].Split(',').Select(int.Parse).ToArray();
                var instruction = input[i + 1].Split(' ').Select(int.Parse).ToArray();
                var after = input[i + 2][9..^1].Split(',').Select(int.Parse).ToArray();
                yield return (before, (instruction[0], instruction[1], instruction[2], instruction[3]), after);
            }
            else
            {
                break;
            }
        }
    }

}