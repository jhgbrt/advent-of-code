namespace AdventOfCode.Year2017.Day22;

static class Extensions
{
    public static IEnumerable<string> ReadLines(this string s)
    {
        using (var reader = new StringReader(s))
        {
            foreach (var line in reader.ReadLines()) yield return line;
        }
    }

    public static IEnumerable<string> ReadLines(this TextReader reader)
    {
        while (reader.Peek() >= 0) yield return reader.ReadLine();
    }

    public static char[,] ToRectangular(this IEnumerable<string> lines)
        => lines.Select(s => s.ToCharArray()).ToArray().ToRectangular();

    public static string[] FromRectangular(this char[,] array)
        => array.ToJagged().Select(s => new string(s)).ToArray();

    static T[][] ToJagged<T>(this T[,] input)
    {
        var rows = input.GetUpperBound(0) + 1;
        var cols = input.GetUpperBound(1) + 1;
        var result = new T[rows][];
        for (var i = 0; i < rows; i++)
        {
            result[i] = new T[cols];
            for (var j = 0; j < cols; j++)
            {
                result[i][j] = input[i, j];
            }
        }
        return result;
    }

    static T[,] ToRectangular<T>(this T[][] arrays)
    {
        int length = arrays.Max(a => a.Length);
        T[,] ret = new T[arrays.Length, length];
        for (int i = 0; i < arrays.Length; i++)
        {
            var array = arrays[i];
            for (int j = 0; j < arrays[i].Length; j++)
            {
                ret[i, j] = array[j];
            }
        }
        return ret;
    }

    public static IEnumerable<(int row, int col, T item)> Enumerate<T>(this T[,] input)
    {
        var rows = input.GetUpperBound(0) + 1;
        var cols = input.GetUpperBound(1) + 1;
        for (var row = 0; row < rows; row++)
        {
            for (var col = 0; col < cols; col++)
            {
                yield return (row, col, input[row, col]);
            }
        }

    }
}

