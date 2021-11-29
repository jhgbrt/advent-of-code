namespace AdventOfCode.Year2020.Day23.Part1;
public static class Runner
{
    public static object Run()
    {
        var input = "158937462".Select(c => (int)char.GetNumericValue(c)).ToArray();

        // Consider array as linked list (value at index is the index of the next node)
        // Node is an variation of the FlyWeight pattern

        var linkedlist = new int[input.Length + 1];
        var current = input[0];
        foreach (var value in input)
        {
            linkedlist[current] = current = value;
        }
        linkedlist[current] = input[0];

        var node = linkedlist.NodeAt(input[0]);
        for (int round = 0; round < 100; round++)
        {
            var d = node.Index - 1;
            while (d < 1 || d == node.Value || d == node.Next.Value || d == node.Next.Next.Value)
            {
                d = d < 1 ? input.Length : d - 1;
            }
            var destination = node.JumpTo(d);
            var (first, last) = (node.Next, node.Next.Next.Next);
            node.Next = last.Next;
            last.Next = destination.Next;
            destination.Next = first;
            node = node.Next;
        }

        var sb = new StringBuilder();
        for (node = linkedlist.NodeAt(1); node.Value != 1; node = node.Next)
            sb.Append(node.Value);
        return sb.ToString();

    }
}

static class NodeFactory
{
    public static Node NodeAt(this int[] linkedlist, int i) => new (linkedlist, i);
}
struct Node
{
    public Node(int[] list, int index)
    {
        List = list;
        Index = index;
    }
    readonly int[] List;
    public readonly int Index;
    public int Value => List[Index];
    public Node Next
    {
        get => new (List, Value);
        set => List[Index] = value.Index;
    }
    public Node JumpTo(int destination) => new (List, destination);
    public override string ToString() => (Index, Value).ToString();
}