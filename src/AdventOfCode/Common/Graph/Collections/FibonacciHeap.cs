using System.Diagnostics;
namespace Net.Code.Graph.Collections;

/// <summary>
/// Specifies the order in which a Heap will Dequeue items.
/// </summary>
public enum HeapDirection : short
{
    /// <summary>
    /// Items are Dequeued in Increasing order from least to greatest.
    /// </summary>
    Increasing = 1,
    /// <summary>
    /// Items are Dequeued in Decreasing order, from greatest to least.
    /// </summary>
    Decreasing = -1
}

static class LambdaHelpers
{
    public static void ForEach<T>(this IEnumerable<T> collection, Action<T> action)
    {
        foreach (T item in collection)
        {
            action(item);
        }
    }

}

public sealed class FibonacciHeapLinkedList<TPriority, TValue>
    : IEnumerable<FibonacciHeapCell<TPriority, TValue>>
{
    FibonacciHeapCell<TPriority, TValue>? first;
    FibonacciHeapCell<TPriority, TValue>? last;

    public FibonacciHeapCell<TPriority, TValue>? First => first;

    internal FibonacciHeapLinkedList()
    {
        first = null;
        last = null;
    }

    internal void MergeLists(FibonacciHeapLinkedList<TPriority, TValue> other)
    {
        if (other.first == null)
            return;

        if (first == null)
        {
            first = other.first;
            last = other.last;
            return;
        }

        if (last != null)
        {
            last.Next = other.first;
        }
        other.first.Previous = last;
        last = other.last;
        if (first == null)
        {
            first = other.first;
        }
    }

    internal void AddLast(FibonacciHeapCell<TPriority, TValue> node)
    {
        if (last != null)
        {
            last.Next = node;
        }
        node.Previous = last;
        last = node;
        if (first == null)
        {
            first = node;
        }
    }

    internal void Remove(FibonacciHeapCell<TPriority, TValue> node)
    {
        if (node.Previous != null)
        {
            node.Previous.Next = node.Next;
        }
        else if (first == node)
        {
            first = node.Next;
        }

        if (node.Next != null)
        {
            node.Next.Previous = node.Previous;
        }
        else if (last == node)
        {
            last = node.Previous;
        }

        node.Next = null;
        node.Previous = null;
    }


    public IEnumerator<FibonacciHeapCell<TPriority, TValue>> GetEnumerator()
    {
        var current = first;
        while (current != null)
        {
            yield return current;
            current = current.Next;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

    internal void Clear()
    {
        first = null;
        last = null;
    }
}

public sealed class FibonacciHeapCell<TPriority, TValue> (TPriority priority, TValue value)
{
    /// <summary>
    /// Determines of a Node has had a child cut from it before
    /// </summary>
    internal bool Marked;
    /// <summary>
    /// Determines the depth of a node
    /// </summary>
    internal int Degree = 1;
    public TPriority Priority { get; internal set; } = priority;
    public TValue Value { get; private set; } = value;
    public bool Removed;
    public FibonacciHeapLinkedList<TPriority, TValue> Children { get; } = [];
    public FibonacciHeapCell<TPriority, TValue>? Parent { get; internal set; }
    public FibonacciHeapCell<TPriority, TValue>? Next { get; internal set; }
    public FibonacciHeapCell<TPriority, TValue>? Previous { get; internal set; }
    public KeyValuePair<TPriority, TValue> ToKeyValuePair() => new(Priority, Value);
}

[DebuggerDisplay("Count = {Count}")]
public sealed class FibonacciHeap<TPriority, TValue>(HeapDirection direction, Func<TPriority, TPriority, int> priorityComparison)
    : IEnumerable<KeyValuePair<TPriority, TValue>>
    where TPriority: struct
{
    public FibonacciHeap()
        : this(HeapDirection.Increasing, Comparer<TPriority>.Default.Compare)
    { }

    public FibonacciHeap(HeapDirection direction)
        : this(direction, Comparer<TPriority>.Default.Compare)
    { }

    readonly FibonacciHeapLinkedList<TPriority, TValue> nodes = [];
    readonly private short DirectionMultiplier = (short)direction;  // Used to control the direction of the heap, set to 1 if the Heap is increasing, -1 if it's decreasing
                                                                                                           // We use the approach to avoid unnessecary branches
    readonly private Dictionary<int, FibonacciHeapCell<TPriority, TValue>> degreeToNode = [];
    private FibonacciHeapCell<TPriority, TValue>? top;

    public HeapDirection Direction { get; } = direction;
    public int Count { get; private set; } = 0;

    readonly record struct NodeLevel(FibonacciHeapCell<TPriority, TValue> Node, int Level);

    public Func<TPriority, TPriority, int> PriorityComparison { get; } = priorityComparison;

    //Draws the current heap in a string.  Marked Nodes have a * Next to them
    public override string ToString()
    {
        var lines = new List<string>();
        var lineNum = 0;
        var columnPosition = 0;
        var list = new List<NodeLevel>();
        foreach (var node in nodes) list.Add(new NodeLevel(node, 0));
        list.Reverse();
        var stack = new Stack<NodeLevel>(list);
        while (stack.Count > 0)
        {
            var currentcell = stack.Pop();
            lineNum = currentcell.Level;
            if (lines.Count <= lineNum)
                lines.Add(string.Empty);
            var currentLine = lines[lineNum];
            currentLine = currentLine.PadRight(columnPosition, ' ');
            var nodeString = currentcell.Node.Priority + (currentcell.Node.Marked ? "*" : "") + " ";
            currentLine += nodeString;
            if (currentcell.Node.Children != null && currentcell.Node.Children.First != null)
            {
                var children = new List<FibonacciHeapCell<TPriority, TValue>>(currentcell.Node.Children);
                children.Reverse();
                children.ForEach(x => stack.Push(new NodeLevel(x, currentcell.Level + 1)));
            }
            else
            {
                columnPosition += nodeString.Length;
            }
            lines[lineNum] = currentLine;
        }
        return string.Join(Environment.NewLine, lines);
    }

    public FibonacciHeapCell<TPriority, TValue> Enqueue(TPriority Priority, TValue Value)
    {
        var newNode = new FibonacciHeapCell<TPriority, TValue>(Priority, Value);

        //We don't do any book keeping or maintenance of the heap on Enqueue,
        //We just add this node to the end of the list of Heaps, updating the Next if required
        nodes.AddLast(newNode);
        if (top == null ||
            (PriorityComparison(newNode.Priority, top.Priority) * DirectionMultiplier) < 0)
        {
            top = newNode;
        }
        Count++;
        return newNode;
    }

    public void Delete(FibonacciHeapCell<TPriority, TValue> node)
    {
        ChangeKeyInternal(node, default, true);
        Dequeue();
    }

    public void ChangeKey(FibonacciHeapCell<TPriority, TValue> node, TPriority newKey)
    {
        ChangeKeyInternal(node, newKey, false);
    }

    private void ChangeKeyInternal(
        FibonacciHeapCell<TPriority, TValue> node,
        TPriority newKey, bool deletingNode)
    {
        var delta = Math.Sign(PriorityComparison(node.Priority, newKey));
        if (delta == 0)
            return;
        if (delta == DirectionMultiplier || deletingNode)
        {
            //New value is in the same direciton as the heap
            node.Priority = newKey;
            if (node.Parent != null && ((PriorityComparison(newKey, node.Parent.Priority) * DirectionMultiplier) < 0 || deletingNode))
            {
                node.Marked = false;
                if (node.Parent.Children is null) throw new NullReferenceException("expecting parent children to be not null");
                node.Parent.Children?.Remove(node);
                var parentNode = node.Parent;
                UpdateNodesDegree(node.Parent);
                node.Parent = null;
                nodes.AddLast(node);
                //This loop is the cascading cut, we continue to cut
                //ancestors of the node reduced until we hit a root 
                //or we found an unmarked ancestor
                while (parentNode.Marked && parentNode.Parent != null)
                {
                    if (parentNode.Parent.Children is null) throw new NullReferenceException("expecting parent children to be not null");
                    parentNode.Parent.Children.Remove(parentNode);
                    UpdateNodesDegree(parentNode);
                    parentNode.Marked = false;
                    nodes.AddLast(parentNode);
                    var currentParent = parentNode;
                    parentNode = parentNode.Parent;
                    currentParent.Parent = null;
                }
                if (parentNode.Parent != null)
                {
                    //We mark this node to note that it's had a child
                    //cut from it before
                    parentNode.Marked = true;
                }
            }
            //Update next
            if (deletingNode || (PriorityComparison(newKey, Top.Priority) * DirectionMultiplier) < 0)
            {
                top = node;
            }
        }
        else
        {
            //New value is in opposite direction of Heap, cut all children violating heap condition
            node.Priority = newKey;
            if (node.Children != null)
            {
                var toupdate = (from child in node.Children
                                where (PriorityComparison(node.Priority, child.Priority) * DirectionMultiplier) > 0
                                select child).ToList();

                foreach (var child in toupdate)
                {
                    node.Marked = true;
                    node.Children.Remove(child);
                    child.Parent = null;
                    child.Marked = false;
                    nodes.AddLast(child);
                    UpdateNodesDegree(node);
                }
            }
            UpdateNext();
        }
    }

    /// <summary>
    /// Updates the degree of a node, cascading to update the degree of the
    /// parents if nessecary
    /// </summary>
    /// <param name="parentNode"></param>
    private void UpdateNodesDegree(
        FibonacciHeapCell<TPriority, TValue> parentNode)
    {
        var oldDegree = parentNode.Degree;

        parentNode.Degree = parentNode.Children.First != null ? parentNode.Children.Max(x => x.Degree) + 1 : 1;

        if (oldDegree != parentNode.Degree)
        {
            if (degreeToNode.TryGetValue(oldDegree, out var value) && value == parentNode)
            {
                degreeToNode.Remove(oldDegree);
            }
            else if (parentNode.Parent != null)
            {
                UpdateNodesDegree(parentNode.Parent);
            }
        }
    }

    public KeyValuePair<TPriority, TValue> Dequeue()
    {
        if (Count == 0 || top is null)
            throw new InvalidOperationException();

        var result = new KeyValuePair<TPriority, TValue>(
            top.Priority,
            top.Value);

        nodes.Remove(Top);
        top.Next = null;
        top.Parent = null;
        top.Previous = null;
        top.Removed = true;
        if (degreeToNode.TryGetValue(top.Degree, out var currentDegreeNode) && currentDegreeNode == Top)
        {
            degreeToNode.Remove(top.Degree);
        }
        foreach (var child in top.Children)
        {
            child.Parent = null;
        }
        nodes.MergeLists(top.Children);
        top.Children.Clear();
        Count--;
        UpdateNext();

        return result;
    }

    /// <summary>
    /// Updates the Next pointer, maintaining the heap
    /// by folding duplicate heap degrees into eachother
    /// Takes O(lg(N)) time amortized
    /// </summary>
    private void UpdateNext()
    {
        if (top == null)
            throw new InvalidOperationException();
        CompressHeap();
        var node = nodes.First;
        top = nodes.First;
        while (node != null)
        {
            if ((PriorityComparison(node.Priority, top!.Priority) * DirectionMultiplier) < 0)
            {
                top = node;
            }
            node = node.Next;
        }
    }

    private void CompressHeap()
    {
        var node = nodes.First;
        while (node != null)
        {
            var nextNode = node.Next;
            while (degreeToNode.TryGetValue(node.Degree, out var currentDegreeNode) && currentDegreeNode != node)
            {
                degreeToNode.Remove(node.Degree);
                if ((PriorityComparison(currentDegreeNode.Priority, node.Priority) * DirectionMultiplier) <= 0)
                {
                    if (node == nextNode)
                    {
                        nextNode = node.Next;
                    }
                    ReduceNodes(currentDegreeNode, node);
                    node = currentDegreeNode;
                }
                else
                {
                    if (currentDegreeNode == nextNode)
                    {
                        nextNode = currentDegreeNode.Next;
                    }
                    ReduceNodes(node, currentDegreeNode);
                }
            }
            degreeToNode[node.Degree] = node;
            node = nextNode;
        }
    }

    /// <summary>
    /// Given two nodes, adds the child node as a child of the parent node
    /// </summary>
    /// <param name="parentNode"></param>
    /// <param name="childNode"></param>
    private void ReduceNodes(
        FibonacciHeapCell<TPriority, TValue> parentNode,
        FibonacciHeapCell<TPriority, TValue> childNode)
    {
        nodes.Remove(childNode);
        parentNode.Children.AddLast(childNode);
        childNode.Parent = parentNode;
        childNode.Marked = false;
        if (parentNode.Degree == childNode.Degree)
        {
            parentNode.Degree += 1;
        }
    }

    public bool IsEmpty => nodes.First == null;
    public FibonacciHeapCell<TPriority, TValue> Top { get => top ?? throw new InvalidOperationException("Queue is empty"); }

    public void Merge(FibonacciHeap<TPriority, TValue> other)
    {
        if (other.Direction != Direction)
        {
            throw new Exception("Error: Heaps must go in the same direction when merging");
        }
        if (other.top is null)
        {
            return;
        }
        nodes.MergeLists(other.nodes);
        if (top is null || (PriorityComparison(other.top.Priority, top.Priority) * DirectionMultiplier) < 0)
        {
            top = other.Top;
        }
        Count += other.Count;
    }

    public IEnumerator<KeyValuePair<TPriority, TValue>> GetEnumerator()
    {
        var tempHeap = new FibonacciHeap<TPriority, TValue>(Direction, PriorityComparison);
        var nodeStack = new Stack<FibonacciHeapCell<TPriority, TValue>>();
        nodes.ForEach(x => nodeStack.Push(x));
        while (nodeStack.Count > 0)
        {
            var topNode = nodeStack.Peek();
            tempHeap.Enqueue(topNode.Priority, topNode.Value);
            nodeStack.Pop();
            topNode.Children.ForEach(x => nodeStack.Push(x));
        }
        while (!tempHeap.IsEmpty)
        {
            yield return tempHeap.Top.ToKeyValuePair();
            tempHeap.Dequeue();
        }
    }
    public IEnumerable<KeyValuePair<TPriority, TValue>> GetDestructiveEnumerator()
    {
        while (!IsEmpty)
        {
            yield return Top.ToKeyValuePair();
            Dequeue();
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}