namespace AdventOfCode.Year2018.Day21;

static class Ex
{
    public static IEnumerable<Instruction> GetInstructions(this string[] input)
    {
        for (int i = 1; i < input.Length; i++)
        {
            var instruction = input[i].Split(' ');
            yield return new(i - 1, instruction[0], int.Parse(instruction[1]), int.Parse(instruction[2]), int.Parse(instruction[3]));
        }
    }
}