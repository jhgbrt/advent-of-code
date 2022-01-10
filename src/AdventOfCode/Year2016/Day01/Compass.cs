namespace AdventOfCode.Year2016.Day01;

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