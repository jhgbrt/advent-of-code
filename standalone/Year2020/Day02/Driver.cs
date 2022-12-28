namespace AdventOfCode.Year2020.Day02;

static class Driver
{
    public static Regex LineRegex = new(@"(?<Min>\d+)-(?<Max>\d+) (?<Letter>\w): (?<Password>\w+)", RegexOptions.Compiled);

    public static bool IsValid1(Entry entry)
    {
        var n = entry.Password.Count(x => x == entry.Letter);
        return n >= entry.Min && n <= entry.Max;
    }

    public static bool IsValid2(Entry entry)
    {
        return entry.Password[entry.Min - 1] == entry.Letter ^ entry.Password[entry.Max - 1] == entry.Letter;
    }

    public static Entry ToEntry(string input)
    {
        var match = LineRegex.Match(input);
        var groups = match.Groups;
        return new Entry(
            int.Parse(groups["Min"].Value),
            int.Parse(groups["Max"].Value),
            groups["Letter"].Value.Single(),
            groups["Password"].Value
            );
    }

    public static long Part1(IEnumerable<string> input)
        => input
        .Select(ToEntry)
        .Where(e => IsValid1(e))
        .Count();

    public static long Part2(IEnumerable<string> input)
        => input
        .Select(ToEntry)
        .Where(e => IsValid2(e))
        .Count();
}