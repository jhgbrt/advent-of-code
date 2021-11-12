using AdventOfCode;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string input = File.ReadAllText("input.txt");

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));

    public static int Part1(string input) => ToTree(input).AllNodes().SelectMany(n => n.MetaData).Sum();

    public static int Part2(string input) => ToTree(input).GetValue();

    static Node ToTree(string input)
    {
        var enumerator = input.ToIntegers().GetEnumerator();
        var root = ReadNode(enumerator);
        return root;
    }

    static Node ReadNode(IEnumerator<int> enumerator)
    {
        var nofchildren = enumerator.Next();
        var nofmetadata = enumerator.Next();
        var children = Enumerable.Range(0, nofchildren).Select(i => ReadNode(enumerator)).ToList();
        var metadata = enumerator.Read(nofmetadata).ToList();
        return new Node(children, metadata);
    }

}
static class Ex
{
    public static T Next<T>(this IEnumerator<T> enumerator)
    {
        enumerator.MoveNext();
        return enumerator.Current;
    }

    public static IEnumerable<T> Read<T>(this IEnumerator<T> enumerator, int n)
        => Enumerable.Range(0, n).Select(i => enumerator.Next());
    public static IEnumerable<int> ToIntegers(this string input)
    {
        var sb = new StringBuilder();
        foreach (var c in input)
        {
            if (char.IsDigit(c))
            {
                sb.Append(c);
            }
            else
            {
                yield return int.Parse(sb.ToString());
                sb.Clear();
            }
        }
        if (sb.Length > 0)
            yield return int.Parse(sb.ToString());
    }

}

