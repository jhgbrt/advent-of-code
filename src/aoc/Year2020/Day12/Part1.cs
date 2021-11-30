namespace AdventOfCode.Year2020.Day12;

class Part1
{
    public static int Run()
    {
        var input = from line in Read.InputLines(typeof(AoC202012))
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

