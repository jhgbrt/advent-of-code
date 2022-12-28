namespace AdventOfCode.Year2016.Day01;

public class AoC201601
{
    public static string input = Read.InputText();

    public object Part1() => Navigate(input).Part1;
    public object Part2() => Navigate(input).Part2;

    internal static Navigator Navigate(string input)
    {
        var navigator = new Navigator();

        var instructions = input.Parse();
        foreach (var (direction, distance) in instructions)
        {
            navigator.Head(direction, distance);
        }

        return navigator;
    }

}



public enum Bearing
{
    N = 0, E, S, W
}
public enum Direction
{
    L, R
}

class Compass
{
    Bearing _bearing;
    public Compass() : this(Bearing.N) { }
    public Compass(Bearing bearing)
    {
        this._bearing = bearing;
    }

    public Bearing Bearing => _bearing;
    public Compass Turn(Direction direction)
    {
        _bearing += direction == Direction.R ? 1 : -1;
        if (_bearing > Bearing.W) _bearing = Bearing.N;
        if (_bearing < Bearing.N) _bearing = Bearing.W;
        return this;
    }
}

static class Extensions
{
    public static IEnumerable<(Direction, int)> Parse(this string input)
    {
        foreach (var item in input.Split(','))
        {
            var step = item.Trim();
            if (Enum.TryParse(step.Substring(0, 1), out Direction direction)
                && int.TryParse(step.Substring(1), out int distance))
                yield return (direction, distance);
        }
    }
}

class Navigator
{
    private readonly Compass _compass = new Compass();
    private readonly HashSet<(int x, int y)> _visits = new HashSet<(int x, int y)>(new[] { (0, 0) });
    private (int x, int y) _position;
    private (int x, int y)? _remember;

    public int Part1 => Math.Abs(_position.x) + Math.Abs(_position.y);
    public int Part2 => _remember.HasValue ? Math.Abs(_remember.Value.x) + Math.Abs(_remember.Value.y) : -1;

    public void Head(Direction direction, int distance)
    {
        _compass.Turn(direction);

        for (int i = 0; i < distance; i++)
        {
            _position = _compass.Bearing switch
            {
                Bearing.N => (_position.x, _position.y + 1),
                Bearing.E => (_position.x + 1, _position.y),
                Bearing.S => (_position.x, _position.y - 1),
                Bearing.W => (_position.x - 1, _position.y),
                _ => throw new Exception()
            };
            if (!_remember.HasValue && _visits.Contains(_position))
                _remember = _position;
            _visits.Add(_position);
        }
    }

}