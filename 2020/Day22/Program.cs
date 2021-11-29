using System.Collections;
using static AdventOfCode.Year2020.Day22.AoC.Input;
using static AdventOfCode.Year2020.Day22.AoC;

#if DEBUG
Trace.Listeners.Add(new ConsoleTraceListener());
#endif


Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day22
{
    partial class AoC
    {

        internal static Result Part1() => Run(() =>
        {
            var (deck1, deck2) = (new Deck(Deck1, 1), new Deck(Deck2, 2));
            return Play1(deck1, deck2).Reverse().Select((n, i) => n * (i + 1)).Sum();
        });
        internal static Result Part2() => Run(() =>
        {
            var (deck1, deck2) = (new Deck(Deck1, 1), new Deck(Deck2, 2));
            return Play2(deck1, deck2, 1).Reverse().Select((n, i) => n * (i + 1)).Sum();
        });


        public static Deck Play1(Deck d1, Deck d2)
        {
            while (d1.Any() && d2.Any())
            {
#if DEBUG
            Trace.WriteLine($"player 1: {d1}");
            Trace.WriteLine($"player 2: {d2}");
#endif
                (d1, d2) = (d1.Dequeue(out int c1), d2.Dequeue(out int c2));
                var winner = c1 > c2 ? d1 : d2;
                (d1, d2) = winner switch
                {
                    { Player: 1 } => (d1.Enqueue(c1).Enqueue(c2), d2),
                    { Player: 2 } => (d1, d2.Enqueue(c2).Enqueue(c1)),
                    _ => throw new()
                };
#if DEBUG
            Trace.WriteLine($"player {winner} wins");
#endif
            }
            return d1.Any() ? d1 : d2;
        }
        public static Deck Play2(Deck d1, Deck d2, int level)
        {
            int round = 0;
            var seen = ImmutableHashSet<(Deck p1, Deck p2)>.Empty;
            while (d1.Any() && d2.Any())
            {
                round++;
                if (seen.Contains((d1, d2)))
                {
#if DEBUG
                Trace.WriteLine($"seen: {(d1, d2)}");
#endif
                    return d1;
                }
                seen = seen.Add((d1, d2));

#if DEBUG
            Trace.WriteLine($"--Round: {round} (Game {level})--");
            Trace.WriteLine($"player 1: {d1}");
            Trace.WriteLine($"player 2: {d2}");
#endif
                (d1, d2) = (d1.Dequeue(out int c1), d2.Dequeue(out int c2));
#if DEBUG
            Trace.WriteLine($"player 1 plays {c1}");
            Trace.WriteLine($"player 2 plays {c2}");
#endif
                Deck winner;
                if (d1.Count() >= c1 && d2.Count() >= c2)
                {
#if DEBUG
                Trace.WriteLine("Subgame needed to determine winner");
#endif
                    winner = Play2(new Deck(d1.Take(c1), d1.Player), new Deck(d2.Take(c2), d2.Player), level + 1);
#if DEBUG
                Trace.WriteLine($"back to game {level}");
#endif
                }
                else
                {
                    winner = c1 > c2 ? d1 : d2;
                }
                (d1, d2) = winner switch
                {
                    { Player: 1 } => (d1.Enqueue(c1).Enqueue(c2), d2),
                    { Player: 2 } => (d1, d2.Enqueue(c2).Enqueue(c1)),
                    _ => throw new()
                };
#if DEBUG
            Trace.WriteLine($"Player {winner} wins round {round} of game {level}");
            Trace.WriteLine(string.Empty);
#endif
            }
            return d1.Any() ? d1 : d2;
        }


        public static class Input
        {
            public static int[] Deck1 = new[]
            {
            14, 23, 6 , 16, 46, 24, 13, 25, 17, 4 , 31, 7 , 1 , 47, 15, 9 , 50, 3 , 30, 37, 43, 10, 28, 33, 32
        };
            public static int[] Deck2 = new[]
            {
            29, 49, 11, 42, 35, 18, 39, 40, 36, 19, 48, 22, 2 , 20, 26, 8 , 12, 44, 45, 21, 38, 41, 34, 5 , 27
        };

        }
        public static class TestRecursion
        {
            public static int[] Deck1 = new[] { 43, 19 };
            public static int[] Deck2 = new[] { 2, 29, 14 };
        }
        public static class Example
        {
            public static int[] Deck1 = new[]
            {
            9,2,6,3,1
        };
            public static int[] Deck2 = new[]
            {
            5,8,4,7,10
        };
        }
    }
}

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