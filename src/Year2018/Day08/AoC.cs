namespace AdventOfCode.Year2018.Day08;

public class AoCImpl : AoCBase
{
    static string input = Read.InputText(typeof(AoCImpl));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);

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