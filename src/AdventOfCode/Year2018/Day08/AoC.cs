namespace AdventOfCode.Year2018.Day08;

public class AoC201808
{
    static string input = Read.InputText();

    public object Part1() => Part1(input);
    public object Part2() => Part2(input);

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

class Node
{
    public Node(IReadOnlyList<Node> children, IReadOnlyList<int> metadata)
    {
        MetaData = metadata;
        Children = children;
    }
    public IReadOnlyList<int> MetaData { get; }
    public IReadOnlyList<Node> Children { get; }

    public IEnumerable<Node> AllNodes() => new[] { this }.Concat(Children.SelectMany(c => c.AllNodes()));

    public int GetValue()
        => Children.Any()
            ? ChildrenFromMetaData.Select(child => child.GetValue()).Sum()
            : MetaData.Sum();

    private IEnumerable<Node> ChildrenFromMetaData => from m in MetaData let i = m - 1 where i >= 0 && i < Children.Count select Children[i];
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