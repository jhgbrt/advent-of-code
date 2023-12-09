namespace AdventOfCode.Year2023.Day09;
public class AoC202309
{
    static ImmutableArray<ImmutableArray<int>> input = (from line in Read.InputLines()
                                                        select (
                                                            from s in line.Split(' ')
                                                            select int.Parse(s)
                                                            ).ToImmutableArray()
                                                        ).ToImmutableArray();
    public int Part1() => (from line in input
                           from value in GetValuesAt(line, ^1)
                           select value).Sum();

    public int Part2() => (from line in input
                           let values = GetValuesAt(line, 0).Reverse()
                           select values.Skip(1).Aggregate(0, (a, b) => b - a)).Sum();

    IEnumerable<int> GetValuesAt(IEnumerable<int> line, Index index)
    {
        var sequence = line.ToList();
        yield return sequence[index];
        while (!sequence.All(i => i == 0))
        {
            for (int i = 0; i < sequence.Count - 1; i++)
            {
                sequence[i] = sequence[i + 1] - sequence[i];
            }
            sequence.RemoveAt(sequence.Count - 1);
            yield return sequence[index];
        }
    }


}

