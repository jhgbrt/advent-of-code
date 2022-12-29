namespace AdventOfCode.Year2020.Day23;

public class AoC202023
{
    static string input = Read.InputLines()[0];
    static int[] numbers = input.Select(c => (int)char.GetNumericValue(c)).ToArray();
    public string Part1()
    {
        // Consider array as linked list (value at index is the index of the next node)
        // Node is an variation of the FlyWeight pattern
        return BuildList(numbers[0], numbers.Length, 100)
            .NodeAt(1).AsEnumerable()
            .TakeWhile(n => n.Value != 1)
            .Aggregate(new StringBuilder(), (sb, node) => sb.Append(node.Value)).ToString();
    }
    public long Part2()
    {
        var node = BuildList(numbers[0], 1_000_000, 10_000_000).NodeAt(1);
        return (long)node.Value * node.Next.Value;
    }


    static int[] BuildList(int start, int size, int rounds)
    {
        var linkedlist = new int[size + 1];
        var current = numbers[0];
        foreach (var value in numbers)
        {
            linkedlist[current] = current = value;
        }
        for (int i = numbers.Length + 1; i < linkedlist.Length; i++)
        {
            linkedlist[current] = current = i;
        }
        linkedlist[current] = numbers[0];

        var node = linkedlist.NodeAt(start);
        for (int round = 0; round < rounds; round++)
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
        return linkedlist;
    }

}

static class Ex
{
    public static Node NodeAt(this int[] linkedlist, int i) => new(linkedlist, i);
    public static IEnumerable<Node> AsEnumerable(this Node node)
    {
        while (true)
        {
            yield return node;
            node = node.Next;
        }
    }
}


readonly record struct Node(int[] List, int Index)
{
    public int Value => List[Index];
    public Node Next
    {
        get => new(List, Value);
        set => List[Index] = value.Index;
    }
    public Node JumpTo(int destination) => this with { Index = destination };
    public override string ToString() => (Index, Value).ToString();
}
