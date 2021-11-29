namespace AdventOfCode.Year2020.Day07;

static class Ex
{
    public static IEnumerable<Relation> AllParents(this ILookup<string, Relation> tree, string child)
    {
        var parents = new[] { child }.SelectMany(c => tree[c]);
        while (parents.Any())
        {
            foreach (var parent in parents) yield return parent;
            parents = parents.Select(p => p.Parent).SelectMany(c => tree[c]);
        }
    }
    public static IEnumerable<(int n, Relation item)> AllChildren(this ILookup<string, Relation> tree, string name, int n)
    {
        foreach (var child in new[] { name }.SelectMany(c => tree[c]))
        {
            yield return (n, child);
            foreach (var item in AllChildren(tree, child.Child, n * child.N)) yield return item;
        }
    }
}