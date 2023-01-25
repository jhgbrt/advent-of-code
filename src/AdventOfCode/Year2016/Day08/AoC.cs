namespace AdventOfCode.Year2016.Day08;

public class AoC201608
{

    public static string[] input = Read.InputLines();

    public object Part1() => Run().Count;
    public object Part2() => Run().ToString();

    static Regex rotate = new Regex("rotate (?<op>(row|column)) (x|y)=(?<i>\\d*) by (?<by>\\d*)", RegexOptions.Compiled);
    static Regex rect = new Regex("rect (?<rows>\\d*)x(?<cols>\\d)*", RegexOptions.Compiled);
    static Display Run()
    {
        var display = new Display(6, 50);
        foreach (var line in input)
        {
            var matchRect = rect.Match(line);
            if (matchRect.Success)
            {
                var rows = int.Parse(matchRect.Groups["rows"].ToString());
                var cols = int.Parse(matchRect.Groups["cols"].ToString());
                display.Rect(rows, cols);
            }
            var matchRotate = rotate.Match(line);
            if (matchRotate.Success)
            {

                var op = matchRotate.Groups["op"].ToString();

                var i = int.Parse(matchRotate.Groups["i"].ToString());
                var by = int.Parse(matchRotate.Groups["by"].ToString());
                if (op == "row")
                    display.RotateRow(i, by);
                else
                    display.RotateCol(i, by);
            }
        }
        return display;

    }
}

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

        for (int col = 0; col < display.GetLength(0); col++)
        {
            for (int row = 0; row < display.GetLength(1); row++)
            {
                sb.Append(display[col, row] ? '#' : '.');
            }
            sb.AppendLine();
        }

        return sb.ToString().DecodePixels(AsciiFontSize._4x6);
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

static class Extensions
{
    public static IEnumerable<T> Rotate<T>(this IList<T> input, int offset)
    {
        return input.Skip(input.Count - offset).Concat(input.Take(input.Count - offset));
    }

    public static void RotateRow<T>(this T[,] array, int row, int n)
    {
        array.ReplaceRow(row, array.Row(row).ToList().Rotate(n).ToArray());
    }
    public static void RotateCol<T>(this T[,] array, int col, int n)
    {
        array.ReplaceCol(col, array.Column(col).ToList().Rotate(n).ToArray());
    }

    public static IEnumerable<T> Row<T>(this T[,] array, int row)
    {
        for (int i = 0; i < array.GetLength(1); i++)
            yield return array[row, i];
    }
    public static IEnumerable<T> Column<T>(this T[,] array, int column)
    {
        for (int i = 0; i < array.GetLength(0); i++)
            yield return array[i, column];
    }

    public static void ReplaceRow<T>(this T[,] array, int row, T[] replacement)
    {
        for (int col = 0; col < replacement.Length; col++)
        {
            array[row, col] = replacement[col];
        }
    }
    public static void ReplaceCol<T>(this T[,] array, int col, T[] replacement)
    {
        for (int row = 0; row < replacement.Length; row++)
        {
            array[row, col] = replacement[row];
        }
    }


    public static string Display<T>(this T[,] array, Func<T, string?>? tostring = null)
    {
        var sb = new StringBuilder().AppendLine();
        if (tostring == null) tostring = t => t!.ToString();

        for (int row = 0; row <= array.GetUpperBound(0); row++)
        {
            for (int col = 0; col <= array.GetUpperBound(1); col++)
            {
                sb.Append(tostring(array[row, col]));
            }
            sb.AppendLine();
        }
        return sb.ToString();
    }


}
