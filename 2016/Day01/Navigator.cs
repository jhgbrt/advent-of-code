namespace AdventOfCode.Year2016.Day01;

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