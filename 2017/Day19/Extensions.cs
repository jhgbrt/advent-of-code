namespace AdventOfCode.Year2017.Day19;

static class Extensions
{
    public static IEnumerable<string> ReadLines(this string s)
    {
        using (var reader = new StringReader(s))
        {
            foreach (var line in reader.ReadLines()) yield return line;
        }
    }

    public static IEnumerable<string> ReadLines(this TextReader reader)
    {
        while (reader.Peek() >= 0) yield return reader.ReadLine();
    }
}