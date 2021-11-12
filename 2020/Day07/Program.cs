
using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string key = "shiny gold";

    internal static Result Part1() => Run(() => Part1("input.txt"));
    internal static Result Part2() => Run(() => Part2("input.txt"));
    static IEnumerable<Relation> relations(string input) =>
            from line in File.ReadLines(input)
            let parent = line[0..line.IndexOf(" bags contain ")]
            from substr in line[(parent.Length + 14)..line.Length].Split(", ")
            let n = substr[0..2] switch
            {
                "no" => 0,
                _ => int.Parse(substr[0..substr.IndexOf(" ")])
            }
            let child = substr[2..].Remove(substr[2..].IndexOf(" bag"))
            select new Relation(parent, n, child);

    internal static int Part1(string input)
        => relations(input).ToLookup(x => x.Child).AllParents(key).Select(x => x.Parent).Distinct().Count();
    internal static int Part2 (string input)
        => relations(input).ToLookup(x => x.Parent).AllChildren(key, 1).Select(x => x.n * x.item.N).Sum();

}

public class Tests
{
    [Fact]
    public void Test1()
    {
    Assert.Equal(4, Part1("sample.txt"));
    }
    [Fact]
    public void Test2()
    {
    Assert.Equal(32, Part2("sample.txt"));
    Assert.Equal(126, Part2("sample2.txt"));
    }
}

record Relation(string Parent, int N, string Child);

static class Ex
{
    public static IEnumerable<Relation> AllParents(this ILookup<string, Relation> tree, string child)
    {
        var parents = new [] {child}.SelectMany(c => tree[c]);
        while (parents.Any())
        {
            foreach (var parent in parents) yield return parent;
            parents = parents.Select(p => p.Parent).SelectMany(c => tree[c]);
        }
    }
    public static IEnumerable<(int n, Relation item)> AllChildren(this ILookup<string, Relation> tree, string name, int n)
    {
        foreach (var child in new [] {name}.SelectMany(c => tree[c]))
        {
            yield return (n, child);
            foreach (var item in AllChildren(tree, child.Child, n*child.N)) yield return item;
        }
    }
}
public class Test
{

}