namespace AdventOfCode.Year2017.Day21;

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

    public static T[,] Transform<T>(this T[,] src, int rotations, bool flip)
    {
        T[,] result = src;
        for (int i = 0; i < rotations; i++)
            result = result.Rotate();
        if (flip)
            result = result.Flip();
        return result;
    }

    static T[,] Rotate<T>(this T[,] src)
    {
        var width = src.GetUpperBound(0) + 1;
        var height = src.GetUpperBound(1) + 1;
        var dst = new T[height, width];
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                dst[height - (row + 1), col] = src[col, row];
            }
        }
        return dst;
    }

    public static T[,] Flip<T>(this T[,] src)
    {
        return src.ToJagged().Select(x => x.Reverse().ToArray()).ToArray().ToRectangular();
    }

    public static bool AllEqual(this char[,] src, char[,] other)
    {
        if (src.GetUpperBound(0) != other.GetUpperBound(0)
            || src.GetUpperBound(1) != other.GetUpperBound(1))
            return false;
        var width = src.GetUpperBound(0) + 1;
        var height = src.GetUpperBound(1) + 1;
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < width; col++)
            {
                if (other[col, row] != src[col, row]) return false;
            }
        }
        return true;
    }

    public static bool IsSquare<T>(this T[,] src) => src.GetUpperBound(0) == src.GetUpperBound(1);

    public static IEnumerable<T[,]> Squares<T>(this T[,] src, int size)
    {
        if (!src.IsSquare())
            throw new Exception("should be square");
        var outerSize = src.GetUpperBound(0) + 1;
        if (outerSize % size != 0) throw new Exception($"{outerSize} can't be divided in squares of size {size}");

        var n = outerSize / size;
        for (var rowOffset = 0; rowOffset < n; rowOffset++)
            for (var colOffset = 0; colOffset < n; colOffset++)
            {
                var result = new T[size, size];
                for (int row = 0; row < size; row++)
                    for (int col = 0; col < size; col++)
                        result[col, row] = src[col + colOffset * size, row + rowOffset * size];
                yield return result;
            }
    }

    public static T[,] Assemble<T>(this IEnumerable<T[,]> items, int smallSquareSize, int totalSquareSize)
    {
        var result = new T[totalSquareSize, totalSquareSize];
        using (var enumerator = items.GetEnumerator())
        {
            for (var rowOffset = 0; rowOffset < totalSquareSize / smallSquareSize; rowOffset++)
                for (var colOffset = 0; colOffset < totalSquareSize / smallSquareSize; colOffset++)
                {
                    enumerator.MoveNext();
                    var input = enumerator.Current;
                    for (int row = 0; row < smallSquareSize; row++)
                        for (int col = 0; col < smallSquareSize; col++)
                        {
                            result[col + colOffset * smallSquareSize, row + rowOffset * smallSquareSize] = input[col, row];
                        }
                }
        }
        return result;
    }

    public static T[,] Repeat<T>(this T[,] input, int i)
    {
        if (!input.IsSquare())
            throw new Exception("should be square");
        var size = input.GetUpperBound(0) + 1;
        var result = new T[size * i, size * i];
        for (var rowOffset = 0; rowOffset < i; rowOffset++)
            for (var colOffset = 0; colOffset < i; colOffset++)
            {
                for (int row = 0; row < size; row++)
                    for (int col = 0; col < size; col++)
                        result[col + colOffset * size, row + rowOffset * size] = input[col, row];
            }
        return result;
    }
}