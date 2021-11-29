using static System.Math;

namespace AdventOfCode.Year2020.Day12;

class Part2
{
    public static int Run()
    {

        var input = from line in File.ReadLines("input.txt")
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
