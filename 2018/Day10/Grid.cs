namespace AdventOfCode
{
    class Grid
    {
        Point[] _points;

        public Grid(IEnumerable<Point> points, int ticks = 0)
        {
            _points = points.ToArray();
            Ticks = ticks;
        }

        public Grid Move(int ticks) => new Grid(_points.Select(p => p.Move(ticks)), Ticks + ticks);

        int MinX => _points.Min(p => p.X);
        int MaxX => _points.Max(p => p.X);
        int MinY => _points.Min(p => p.Y);
        int MaxY => _points.Max(p => p.Y);
        public int Ticks { get; }
        public int Width => MaxX - MinX + 1;
        public int Height => MaxY - MinY + 1;

        public override string ToString()
        {
            var grid = Enumerable.Range(0, Height)
                .Select(i => Enumerable.Repeat('.', Width).ToArray())
                .ToArray();

            foreach (var point in _points)
            {
                grid[point.Y - MinY][point.X - MinX] = '#';
            }

            return string.Join(Environment.NewLine, grid.Select(a => new string(a)));
        }
        public string Decode()
        {
            var grid = new char[Height,Width];
            for (int r = 0; r < Height; r++)
                for (int c = 0; c < Width; c++)
                    grid[r, c] = '.';

            foreach (var point in _points)
            {
                grid[point.Y - MinY, point.X - MinX] = '#';
            }

            var sb = new StringBuilder();
            for (int col = 0; col < grid.GetLength(1); col += 8)
            {
                sb.Append(GetLetter(0, col, grid));
            }
            return sb.ToString();// display.Display(b => b ? "#" : ".");

        }

        static string s = @"
######..#####.....##....#####...#....#..#....#.....###...####.
.....#..#....#...#..#...#....#..#....#..#....#......#...#....#
.....#..#....#..#....#..#....#...#..#....#..#.......#...#.....
....#...#....#..#....#..#....#...#..#....#..#.......#...#.....
...#....#####...#....#..#####.....##......##........#...#.....
..#.....#..#....######..#....#....##......##........#...#.....
.#......#...#...#....#..#....#...#..#....#..#.......#...#.....
#.......#...#...#....#..#....#...#..#....#..#...#...#...#.....
#.......#....#..#....#..#....#..#....#..#....#..#...#...#....#
######..#....#..#....#..#####...#....#..#....#...###.....####.";

        char GetLetter(int row, int col, char[,] display)
        {
            var sb = new StringBuilder().AppendLine();
            for (int r = row; r < display.GetLength(0); r++)
            {
                for (int c = col; c < col + 6; c++)
                    sb.Append(display[r,c]);
                sb.AppendLine();
            }

            return sb.ToString() switch
            {
                AsciiLetters.A => 'A',
                AsciiLetters.B => 'B',
                AsciiLetters.C => 'C',
                AsciiLetters.J => 'J',
                AsciiLetters.R => 'R',
                AsciiLetters.X => 'X',
                AsciiLetters.Z => 'Z',
                _ => throw new Exception($"unrecognized letter at ({row}, {col}) ({sb})")
            };
        }

        static class AsciiLetters
        {
            public const string A = @"
..##..
.#..#.
#....#
#....#
#....#
######
#....#
#....#
#....#
#....#
";
            public const string B = @"
#####.
#....#
#....#
#....#
#####.
#....#
#....#
#....#
#....#
#####.
";

            public const string C = @"
.####.
#....#
#.....
#.....
#.....
#.....
#.....
#.....
#....#
.####.
";

            public const string J = @"
...###
....#.
....#.
....#.
....#.
....#.
....#.
#...#.
#...#.
.###..
";
            public const string R = @"
#####.
#....#
#....#
#....#
#####.
#..#..
#...#.
#...#.
#....#
#....#
";
            public const string X = @"
#....#
#....#
.#..#.
.#..#.
..##..
..##..
.#..#.
.#..#.
#....#
#....#
";

            public const string Z = @"
######
.....#
.....#
....#.
...#..
..#...
.#....
#.....
#.....
######
";

        }

    }
}
