namespace AdventOfCode.Year2017.Day25;

public class AoC201725
{
    static string input = Read.InputText();
    public object Part1() => Read.InputText().EncodeToSomethingSimpler().CalculateChecksum();
    public object Part2() => "";
}

static class Turing
{
    public static int CalculateChecksum(this (char beginState, int steps, string code) input)
    {
        var instructions = input.code.ReadLines().Select(line =>
            (
            currentstate: line[0],
            currentvalue: int.Parse(line.Substring(1, 1)),
            newvalue: int.Parse(line.Substring(2, 1)),
            delta: line[3] == '+' ? +1 : -1,
            newstate: line[4])
        ).ToDictionary(x => (x.currentstate, x.currentvalue));

        var tape = new Dictionary<int, int>();

        int GetValue(int i)
        {
            return tape.ContainsKey(i) ? tape[i] : 0;
        }

        var cursor = 0;
        var state = input.beginState;
        for (int i = 0; i < input.steps; i++)
        {
            var value = GetValue(cursor);
            var instruction = instructions[(state, value)];
            tape[cursor] = instruction.newvalue;
            cursor += instruction.delta;
            state = instruction.newstate;
        }

        var actual = tape.Values.Sum();
        return actual;
    }

}

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
        while (reader.Peek() >= 0) yield return reader.ReadLine()!;
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