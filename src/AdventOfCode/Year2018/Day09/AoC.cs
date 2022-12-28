namespace AdventOfCode.Year2018.Day09;

public class AoC201809
{
    static string[] input = Read.InputLines();

    public object Part1() => Part1(465, 71498);
    public object Part2() => Part2(465, 71498);
    public static long Part1(int players, long marbles)
    {
        var game = new Game(players);
        game.Play(marbles);
        return game.HighScore();
    }

    public static long Part2(int players, long marbles) => Part1(players, marbles * 100);

}

class Game
{
    LinkedList<long> _marbles = new LinkedList<long>(new[] { 0L });
    LinkedListNode<long> _current;
    Dictionary<int, long> _scores = new Dictionary<int, long>();
    private int _players;
    private long _marble = 1;

    public Game(int players)
    {
        _players = players;
        _current = _marbles.First ?? throw new Exception();
    }

    public long CurrentMarble => _current?.Value ?? throw new Exception();
    public int CurrentPlayer { get; private set; }

    public void Play(long nofmarbles)
    {
        var max = _marble + nofmarbles;
        for (; _marble < max; _marble++)
        {
            CurrentPlayer = (CurrentPlayer % _players) + 1;
            if (_marble % 23 == 0)
            {
                var toremove = _current.CircularPrevious(7);
                AddScore(CurrentPlayer, _marble + toremove.Value);
                _current = toremove.CircularNext();
                _marbles.Remove(toremove);
            }
            else
            {
                _current = _marbles.AddAfter(_current.CircularNext(), _marble);
            }
        }
    }

    public long this[int player] => _scores[player];

    private void AddScore(int player, long score)
    {
        if (!_scores.ContainsKey(player)) _scores[player] = 0;
        _scores[player] += score;
    }

    public long HighScore() => _scores.Values.Max();
}

public static class LinkedListExt
{
    public static LinkedListNode<T> CircularNext<T>(this LinkedListNode<T> current, int steps = 1)
        => Enumerable.Range(0, steps).Aggregate(current, (c, _) => c.Next ?? c.List?.First ?? throw new Exception());

    public static LinkedListNode<T> CircularPrevious<T>(this LinkedListNode<T> current, int steps = 1)
        => Enumerable.Range(0, steps).Aggregate(current, (c, _) => c.Previous ?? c.List?.Last ?? throw new Exception());
}