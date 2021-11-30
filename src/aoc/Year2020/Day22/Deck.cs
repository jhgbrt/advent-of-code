using System.Collections;

namespace AdventOfCode.Year2020.Day22;

public class Deck : IImmutableQueue<int>, IEquatable<IImmutableQueue<int>>
{
    IImmutableQueue<int> _q;
    public int Player { get; }
    public Deck(IEnumerable<int> cards, int player)
    {
        var q = ImmutableQueue<int>.Empty;
        foreach (var c in cards) q = q.Enqueue(c);
        _q = q;
        Player = player;
    }
    public Deck(IImmutableQueue<int> q, int player)
    {
        _q = q;
        Player = player;
    }

    public Deck Take(int n) => new Deck(_q.Take(n), Player);

    public bool IsEmpty => _q.IsEmpty;

    public IImmutableQueue<int> Clear() => new Deck(_q.Clear(), Player);
    public IImmutableQueue<int> Dequeue() => new Deck(_q.Dequeue(), Player);
    public Deck Dequeue(out int value)
    {
        value = _q.Peek();
        return new Deck(_q.Dequeue(), Player);
    }
    IImmutableQueue<int> IImmutableQueue<int>.Enqueue(int value) => new Deck(_q.Enqueue(value), Player);
    public Deck Enqueue(int value) => new Deck(_q.Enqueue(value), Player);
    public int Peek() => _q.Peek();
    public override bool Equals(object? obj) => obj is not null && Equals((IImmutableQueue<int>)obj);
    public bool Equals(IImmutableQueue<int>? other) => this.SequenceEqual(other ?? ImmutableQueue<int>.Empty);
    public IEnumerator<int> GetEnumerator() => _q.GetEnumerator();

    public override int GetHashCode()
    {
        unchecked
        {
            return this.Aggregate(19, (h, i) => h * 19 + i.GetHashCode());
        }
    }
    IEnumerator IEnumerable.GetEnumerator() => _q.GetEnumerator();

    public override string ToString() => string.Join(",", _q);
}