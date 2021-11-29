namespace AdventOfCode.Year2017.Day25;

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

    public static (char beginState, int checksum, string code) EncodeToSomethingSimpler(this string input)
    {
        var beginState = '\0';
        var checksum = 0;
        var states = new List<string>();
        var sb = new StringBuilder();

        foreach (var line in input.ReadLines())
        {
            if (line.StartsWith("Begin in state"))
            {
                beginState = line[line.Length - 2];
            }
            else if (line.StartsWith("Perform"))
            {
                checksum = int.Parse(line.Split(' ')[5]);
            }
            else if (line.StartsWith("In state "))
            {
                sb.Clear().Append(line[line.Length - 2]);
            }
            else if (line.StartsWith("  If") && sb.Length == 1)
            {
                sb.Append(line[line.Length - 2]);
            }
            else if (line.StartsWith("  If"))
            {
                sb.Remove(1, sb.Length - 1).Append(line[line.Length - 2]);
            }
            else if (line.StartsWith("    - Write"))
            {
                sb.Append(line[line.Length - 2]);
            }
            else if (line.StartsWith("    - Move"))
            {
                sb.Append(line.EndsWith("right.") ? '+' : '-');
            }
            else if (line.StartsWith("    - Continue"))
            {
                sb.Append(line[line.Length - 2]);
                states.Add(sb.ToString());
            }

        }
        return (beginState, checksum, string.Join(Environment.NewLine, states));
    }
}