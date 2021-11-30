namespace AdventOfCode.Year2015.Day19;

public class AoC201519 : AoCBase
{
    static string[] lines = Read.InputLines(typeof(AoC201519));
    static string input = lines.Last();
    static ImmutableList<Replacement> replacements = (
        from line in lines.TakeWhile(line => !string.IsNullOrEmpty(line))
        let split = line.Split(" => ")
        select new Replacement(split[0], split[1])
        ).ToImmutableList();

    public override object Part1() => Part1(input, replacements);
    public override object Part2() => Part2(input);
    static int Part1(string input, ImmutableList<Replacement> replacements)
    {
        HashSet<string> output = new();
        foreach (var r in replacements)
        {
            int position = 0;
            while ((position = input.IndexOf(r.From, position)) >= 0)
            {
                var result = new StringBuilder().Append(input.AsSpan(0, position)).Append(r.To).Append(input.AsSpan(position + r.From.Length)).ToString();
                output.Add(result);
                position += r.From.Length;
            }
        }
        return output.Count;
    }


    static int Part2(string input) => input.Count(char.IsUpper) - Count(input, "Rn") - Count(input, "Ar") - 2 * Count(input, "Y") - 1;

    static int Count(string str, string element)
    {
        var count = 0;
        for (var index = str.IndexOf(element); index >= 0; index = str.IndexOf(element, index + 1), ++count) { }
        return count;
    }
}
record Replacement(string From, string To);