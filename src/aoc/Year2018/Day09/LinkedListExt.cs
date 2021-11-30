namespace AdventOfCode.Year2018.Day09;

public static class LinkedListExt
{
    public static LinkedListNode<T> CircularNext<T>(this LinkedListNode<T> current, int steps = 1)
        => Enumerable.Range(0, steps).Aggregate(current, (c, _) => c.Next ?? c.List?.First ?? throw new Exception());

    public static LinkedListNode<T> CircularPrevious<T>(this LinkedListNode<T> current, int steps = 1)
        => Enumerable.Range(0, steps).Aggregate(current, (c, _) => c.Previous ?? c.List?.Last ?? throw new Exception());
}