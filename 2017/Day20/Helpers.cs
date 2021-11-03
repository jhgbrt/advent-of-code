using System.Collections.Generic;
using System.IO;

static class Helpers
{
    public static IEnumerable<string> ReadLines(this string input)
    {
        using (var reader = new StringReader(input))
        {
            foreach (var line in reader.ReadLines()) yield return line;
        }
    }

    public static IEnumerable<string> ReadLines(this TextReader reader)
    {
        while (reader.Peek() >= 0)
        {
            yield return reader.ReadLine();
        }
    }
}