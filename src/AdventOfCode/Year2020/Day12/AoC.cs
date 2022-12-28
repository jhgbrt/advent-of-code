
using P1 = AdventOfCode.Year2020.Day12.Part1;
using P2 = AdventOfCode.Year2020.Day12.Part2;

namespace AdventOfCode.Year2020.Day12;

public class AoC202012
{
    public object Part1() => P1.Run();
    public object Part2() => P2.Run();
}

static class Part1
{
    public static int Run()
    {
        var input = from line in Read.InputLines()
                    let c = line[0]
                    let i = int.Parse(line.Substring(1))
                    select (c, i);

        var result = input.Aggregate(new State(0, 0, 90), (state, item) => state.Apply(item.c, item.i));

        return result.Distance;

    }
    record State(int X, int Y, int Bearing)
    {
        public State Apply(char c, int n) => c switch
        {
            'F' => Apply(CurrentOrientation, n),
            'L' => this with { Bearing = Turn(-n) },
            'R' => this with { Bearing = Turn(n) },
            'N' => this with { Y = Y + n },
            'E' => this with { X = X + n },
            'S' => this with { Y = Y - n },
            'W' => this with { X = X - n },
            _ => throw new NotImplementedException()
        };

        private int Turn(int degrees) => (Bearing + degrees) switch
        {
            < 0 => Bearing + degrees + 360,
            > 360 => Bearing + degrees - 360,
            _ => Bearing + degrees
        };

        public char CurrentOrientation => (Bearing % 360) switch
        {
            0 => 'N',
            90 => 'E',
            180 => 'S',
            270 => 'W',
            _ => throw new NotImplementedException()
        };
        public int Distance => Math.Abs(X) + Math.Abs(Y);
    }
}

static class Part2
{
    public static int Run()
    {

        var input = from line in Read.InputLines()
                    let c = line[0]
                    let i = int.Parse(line.Substring(1))
                    select (c, i);

        var result = input.Aggregate(new State(0, 0, new(10, 1)), (state, item) => state.Apply(item.c, item.i));
        return result.Distance;
    }
    record Waypoint(int X, int Y)
    {
        public Waypoint Turn(int degrees) => Turn(degrees * PI / 180);
        public Waypoint Turn(double rad) => new((int)Round(R * Cos(Θ + rad)), (int)Round(R * Sin(Θ + rad)));
        public double R => Sqrt(Pow(X, 2) + Pow(Y, 2));
        public double Θ => Atan2(Y, X);
        public Waypoint Move(char c, int n) => c switch
        {
            'N' => this with { Y = Y + n },
            'E' => this with { X = X + n },
            'S' => this with { Y = Y - n },
            'W' => this with { X = X - n },
            _ => throw new NotImplementedException()
        };
    }

    record State(int X, int Y, Waypoint Waypoint)
    {
        public State Apply(char c, int n) => c switch
        {
            'F' => Move(n),
            'L' => this with { Waypoint = Waypoint.Turn(n) },
            'R' => this with { Waypoint = Waypoint.Turn(-n) },
            _ => this with { Waypoint = Waypoint.Move(c, n) }
        };

        State Move(int steps) => this with { X = X + steps * Waypoint.X, Y = Y + steps * Waypoint.Y };

        public int Distance => Abs(X) + Abs(Y);
    }
}
