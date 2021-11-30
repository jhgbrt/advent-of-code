namespace AdventOfCode.Year2018.Day08;

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
