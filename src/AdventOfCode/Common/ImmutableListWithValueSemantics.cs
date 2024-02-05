using System.Collections;

namespace AdventOfCode;


public class ValueList<T> : IEquatable<ValueList<T>>, IEnumerable<T>
{
    readonly ImmutableSortedSet<T> _list;

    public ValueList(IImmutableList<T> list) => _list = list.ToImmutableSortedSet();
    public ValueList(IEnumerable<T> list) => _list = list.ToImmutableSortedSet();

    public T this[int index] => _list[index];
    public IEnumerator<T> GetEnumerator() => _list.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
    public int Count => _list.Count;

    public override bool Equals(object? obj) => Equals(obj as ValueList<T>);
    public bool Equals(ValueList<T>? other) => this.SequenceEqual(other ?? ValueList<T>.Empty);
    public override int GetHashCode()
    {
        unchecked
        {
            return this.Aggregate(19, (h, i) => h * 19 + i!.GetHashCode());
        }
    }
    public override string ToString() => string.Join(",", _list);
    private static readonly ValueList<T> empty = new([]);
    public static ValueList<T> Empty => empty;

}