namespace AdventOfCode.Year2018.Day09;

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
        _current = _marbles.First;
    }

    public long CurrentMarble => _current.Value;
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