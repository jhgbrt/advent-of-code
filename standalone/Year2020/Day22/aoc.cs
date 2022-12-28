using static AdventOfCode.Year2020.Day22.AoC202022.Input;

var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = Part2();
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    var (deck1, deck2) = (new Deck(Deck1, 1), new Deck(Deck2, 2));
    return Play1(deck1, deck2).Reverse().Select((n, i) => n * (i + 1)).Sum();
}

object Part2()
{
    var (deck1, deck2) = (new Deck(Deck1, 1), new Deck(Deck2, 2));
    return Play2(deck1, deck2, 1).Reverse().Select((n, i) => n * (i + 1)).Sum();
}

Deck Play1(Deck d1, Deck d2)
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

Deck Play2(Deck d1, Deck d2, int level)
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
    public static int[] Deck1 { get; } = new[] { 14, 23, 6, 16, 46, 24, 13, 25, 17, 4, 31, 7, 1, 47, 15, 9, 50, 3, 30, 37, 43, 10, 28, 33, 32 };
    public static int[] Deck2 { get; } = new[] { 29, 49, 11, 42, 35, 18, 39, 40, 36, 19, 48, 22, 2, 20, 26, 8, 12, 44, 45, 21, 38, 41, 34, 5, 27 };
}

public static class TestRecursion
{
    public static int[] Deck1 { get; } = new[] { 43, 19 };
    public static int[] Deck2 { get; } = new[] { 2, 29, 14 };
}

public static class Example
{
    public static int[] Deck1 { get; } = new[] { 9, 2, 6, 3, 1 };
    public static int[] Deck2 { get; } = new[] { 5, 8, 4, 7, 10 };
}