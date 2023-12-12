using AdventOfCode.Year2016.Day21;

using System.Collections;

namespace AdventOfCode.Year2020.Day22;

public class AoC202022
{
    Deck deck1;
    Deck deck2;

    public AoC202022() : this(Read.InputLines())
    { }
    public AoC202022(string[] input)
    {
        (deck1, deck2) = Parse(input).ToTuple2();
    }
    IEnumerable<Deck> Parse(IEnumerable<string> input)
    {
        var enumerator = input.GetEnumerator();
        enumerator.MoveNext();
        yield return new Deck(enumerator.While(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToArray(), 1);
        enumerator.MoveNext();
        yield return new Deck(enumerator.While(s => !string.IsNullOrEmpty(s)).Select(int.Parse).ToArray(), 2);
    }
    public object Part1()
    {
        return Play1(deck1, deck2).Reverse().Select((n, i) => n * (i + 1)).Sum();
    }
    public object Part2()
    {
        return Play2(deck1, deck2, 1).Reverse().Select((n, i) => n * (i + 1)).Sum();
    }


    public static Deck Play1(Deck d1, Deck d2)
    {
        while (!d1.IsEmpty && !d2.IsEmpty)
        {
            (d1, d2) = (d1.Dequeue(out int c1), d2.Dequeue(out int c2));
            var winner = c1 > c2 ? d1 : d2;
            (d1, d2) = winner switch
            {
                { Player: 1 } => (d1.Enqueue(c1).Enqueue(c2), d2),
                { Player: 2 } => (d1, d2.Enqueue(c2).Enqueue(c1)),
                _ => throw new()
            };
        }
        return d1.IsEmpty ? d2 : d1;
    }
    public static Deck Play2(Deck d1, Deck d2, int level)
    {
        int round = 0;
        var seen = new HashSet<(Deck p1, Deck p2)>();
        while (!d1.IsEmpty && !d2.IsEmpty)
        {
            round++;
            if (seen.Contains((d1, d2)))
            {
                return d1;
            }
            seen.Add((d1, d2));

            (d1, d2) = (d1.Dequeue(out int c1), d2.Dequeue(out int c2));
            Deck winner;
            if (d1.Count >= c1 && d2.Count >= c2)
            {
                winner = Play2(d1.Take(c1), d2.Take(c2), level + 1);
            }
            else
            {
                winner = c1 > c2 ? d1 : d2;
            }
            (d1, d2) = winner switch
            {
                { Player: 1 } => (d1.Enqueue(c1, c2), d2),
                { Player: 2 } => (d1, d2.Enqueue(c2, c1)),
                _ => throw new()
            };
        }
        return d1.IsEmpty ? d2 : d1;
    }
}

public class Deck 
{
    ImmutableQueue<int> _q;
    public int Player { get; }
    public Deck(IEnumerable<int> cards, int player)
    {
        var q = ImmutableQueue<int>.Empty;
        foreach (var c in cards) q = q.Enqueue(c);
        _q = q;
        Player = player;
    }
    private Deck(ImmutableQueue<int> q, int player)
    {
        _q = q;
        Player = player;
    }

    public Deck Take(int n) => new Deck(_q.Take(n), Player);

    public bool IsEmpty => _q.IsEmpty;

    public Deck Dequeue(out int value) => new (_q.Dequeue(out value), Player);
    public Deck Enqueue(int value) => new (_q.Enqueue(value), Player);
    public Deck Enqueue(int value1, int value2) => new (_q.Enqueue(value1).Enqueue(value2), Player);
    public override bool Equals(object? obj) => obj is Deck other && Equals(other);
    bool Equals(Deck other) => _q.SequenceEqual(other._q);
    public int Count => _q.Count();
    public override int GetHashCode()
    {
        unchecked
        {
            return _q.Peek().GetHashCode();
        }
    }

    public IEnumerable<int> Reverse() => _q.Reverse();

    public override string ToString() => string.Join(",", _q);
}