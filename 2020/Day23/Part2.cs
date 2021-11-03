using System;
using System.Linq;
namespace Part2;
public static class Runner
{
    public static void Run()
    {
        var input = "158937462".Select(c => (int)char.GetNumericValue(c)).ToArray();

        // Consider array as linked list (value at index is the index of the next node)
        // Node is an variation of the FlyWeight pattern

        var size = 1_000_000;
        var linkedlist = new int[size + 1];
        var current = input[0];
        foreach (var value in input)
        {
            linkedlist[current] = current = value;
        }
        for (int i = input.Length + 1; i < linkedlist.Length; i++)
        {
            linkedlist[current] = current = i;
        }
        linkedlist[^1] = input[0];

        var node = linkedlist.NodeAt(input[0]);
        for (int round = 0; round < 10_000_000; round++)
        {
            var d = node.Index - 1;
            while (d < 1 || d == node.Value || d == node.Next.Value || d == node.Next.Next.Value)
            {
                d = d < 1 ? size : d - 1;
            }
            var destination = node.JumpTo(d);
            var (first, last) = (node.Next, node.Next.Next.Next);
            node.Next = last.Next;
            last.Next = destination.Next;
            destination.Next = first;
            node = node.Next;
        }

        node = linkedlist.NodeAt(1);
        Console.WriteLine((long)node.Value * node.Next.Value);

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
        get => new(List, Value);
        set => List[Index] = value.Index;
    }
    public Node JumpTo(int destination) => new (List, destination);
    public override string ToString() => (Index, Value).ToString();
}
