namespace AdventOfCode.Year2018.Day19;

public class AoC201819
{
    static string[] input = Read.InputLines(typeof(AoC201819));

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);
    public static long Part1(string[] input)
    {
        var cpu = new CPU(int.Parse(input[0].Split(' ').Last()), input.GetInstructions(), new[] { 0L, 0, 0, 0, 0, 0 });
        cpu.Run();
        return cpu.Registers[0];
    }

    public static long Part2(string[] input)
    {
        var cpu = new CPU(int.Parse(input[0].Split(' ').Last()), input.GetInstructions(), new[] { 1L, 0, 0, 0, 0, 0 });
        return cpu.RunReverseEngineered().A;
    }
}