class Display
{
    bool[,] display;

    public Display(int rows, int cols)
    {
        display = new bool[rows, cols];
    }

    public int Count
    {
        get { return display.OfType<bool>().Count(b => b); }
    }

    public void Rect(int a, int b)
    {
        for (int row = 0; row < a; row++)
            for (int col = 0; col < b; col++)
            {
                display[col, row] = true;
            }
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        for (int col = 0; col < display.GetLength(1); col+=5)
        {
            sb.Append(GetLetter(0, col));
        }
        return sb.ToString();// display.Display(b => b ? "#" : ".");
    }

    char GetLetter(int row, int col)
    {
        var sb = new StringBuilder().AppendLine();
        for (int r = row; r < display.GetLength(0); r++)
        {
            for (int c = col; c < col+4; c++)
                sb.Append(display[r, c] ? '#' : '.');
            sb.AppendLine();
        }

        return sb.ToString() switch
        {
            AsciiLetters.A => 'A',
            AsciiLetters.B => 'B',
            AsciiLetters.F => 'F',
            AsciiLetters.J => 'J',
            AsciiLetters.P => 'P',
            AsciiLetters.S => 'S',
            AsciiLetters.U => 'U',
            AsciiLetters.Z => 'Z',
            _ => throw new Exception($"unrecognized letter at ({row}, {col}) ({sb})")
        };
    }

    static class AsciiLetters
    {
        public const string A = @"
.##.
#..#
#..#
####
#..#
#..#
";
        public const string B = @"
###.
#..#
###.
#..#
#..#
###.
";

        public const string F = @"
####
#...
###.
#...
#...
#...
";

        public const string J = @"
..##
...#
...#
...#
#..#
.##.
";
        public const string P = @"
###.
#..#
#..#
###.
#...
#...
";
        public const string S = @"
.###
#...
#...
.##.
...#
###.
";
        public const string U = @"
#..#
#..#
#..#
#..#
#..#
.##.
";

        public const string Z = @"
####
...#
..#.
.#..
#...
####
";

    }



    public void RotateCol(int col, int d)
    {
        display.RotateCol(col, d);
    }

    public void RotateRow(int row, int d)
    {
        display.RotateRow(row, d);
    }


}
