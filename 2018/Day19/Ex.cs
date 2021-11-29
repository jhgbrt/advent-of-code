namespace AdventOfCode.Year2018.Day19;

static class Ex
{
    public static IEnumerable<(string code, int a, int b, int c)> GetInstructions(this string[] input)
    {
        for (int i = 1; i < input.Length; i++)
        {
            var instruction = input[i].Split(' ');
            yield return (instruction[0], int.Parse(instruction[1]), int.Parse(instruction[2]), int.Parse(instruction[3]));
        }
    }
}