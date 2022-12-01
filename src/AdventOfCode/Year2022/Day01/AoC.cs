namespace AdventOfCode.Year2022.Day01;
public class AoC202201
{
    static string[] input = Read.InputLines();

    static IEnumerable<IEnumerable<string>> Chunks()
    {
        List<string> chunk = new();
        foreach (var line in input)
        {
            if (string.IsNullOrEmpty(line))
            {
                yield return chunk;
                chunk.Clear();
            }
            else
            {
                chunk.Add(line);
            }
        }
        if (chunk.Any()) yield return chunk;
    }

    public int Part1() => Chunks().Select(chunk => chunk.Select(int.Parse).Sum()).Max();
    public int Part2() => Chunks().Select(chunk => chunk.Select(int.Parse).Sum()).OrderDescending().Take(3).Sum();
}
