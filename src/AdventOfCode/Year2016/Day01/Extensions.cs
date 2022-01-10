namespace AdventOfCode.Year2016.Day01;

static class Extensions
{
    public static IEnumerable<(Direction, int)> Parse(this string input)
    {
        foreach (var item in input.Split(','))
        {
            var step = item.Trim();
            if (Enum.TryParse(step.Substring(0, 1), out Direction direction)
                && int.TryParse(step.Substring(1), out int distance))
                yield return (direction, distance);
        }
    }
}