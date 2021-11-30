namespace AdventOfCode.Year2018.Day06;

static class Ex
{
    private static readonly Regex _regex = new Regex(@"(\d+), (\d+)", RegexOptions.Compiled);
    public static (int x, int y) ToCoordinate(this string input)
    {
        var match = _regex.Match(input);
        return (int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value));
    }

    public static IEnumerable<(int x, int y)> ToCoordinates(this IEnumerable<string> input) => input.Select(ToCoordinate);

}