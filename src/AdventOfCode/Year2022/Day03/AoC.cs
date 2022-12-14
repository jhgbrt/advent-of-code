namespace AdventOfCode.Year2022.Day03;
public class AoC202203
{
    static string[] input = Read.InputLines();
    public int Part1() => (from line in input
                           let half = line.Length / 2
                           let part1 = line[0..half]
                           let part2 = line[half..]
                           from common in part1.Intersect(part2)
                           select Priority(common)).Sum();
    public int Part2() => (from chunk in input.Chunk(3)
                           from common in chunk[0].Intersect(chunk[1]).Intersect(chunk[2]).Distinct()
                           let priority = Priority(common)
                           select priority).Sum();
    private int Priority(char c) => c switch
    {
        >= 'a' and <= 'z' => c - 'a' + 1,
        >= 'A' and <= 'Z' => c - 'A' + 27,
        _ => throw new InvalidOperationException("unexpected character")
    };
}
