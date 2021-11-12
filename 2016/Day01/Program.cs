
using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static bool test = false;
    public static string input = File.ReadAllText(test ? "sample.txt" : "input.txt");

    internal static Result Part1() => Run(() => Navigate(input).Part1);
    internal static Result Part2() => Run(() => Navigate(input).Part2);

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

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(243, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(142, Part2().Value);

    [Theory]
    [InlineData(Bearing.N, Direction.R, Bearing.E)]
    [InlineData(Bearing.E, Direction.R, Bearing.S)]
    [InlineData(Bearing.S, Direction.R, Bearing.W)]
    [InlineData(Bearing.W, Direction.R, Bearing.N)]
    [InlineData(Bearing.N, Direction.L, Bearing.W)]
    [InlineData(Bearing.E, Direction.L, Bearing.N)]
    [InlineData(Bearing.S, Direction.L, Bearing.E)]
    [InlineData(Bearing.W, Direction.L, Bearing.S)]
    public void CompassTests(Bearing current, Direction turn, Bearing expected)
    {
        var compass = new Compass(current).Turn(turn);
        Assert.Equal(expected, compass.Bearing);
    }

    [Theory]
    [InlineData("R2, L3", 5)]
    [InlineData("R2, R2, R2", 2)]
    [InlineData("R5, L5, R5, R3", 12)]
    public void Test(string input, int expected)
    {
        var navigator = Navigate(input);
        var result = navigator.Part1;
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("R1", -1)]
    [InlineData("R5, R5, R5, R5", 0)]
    [InlineData("R5, R4, R4, R4, L1", 1)]
    [InlineData("R8, R4, R4, R8", 4)]
    public void FirstPlaceVisitedTwiceTest(string input, int? expected)
    {
        Navigator navigator = Navigate(input);
        var result = navigator.Part2;
        Assert.Equal(expected, result);
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
