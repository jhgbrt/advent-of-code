namespace AdventOfCode.Year2016.Day09;

public static class Extensions
{
    public static long GetDecompressedSize(this string input, int startIndex)
    {
        long count = 0;
        var i = startIndex;
        while (i < input.Length)
        {
            if (Marker.TryParse(input, i, out Marker result))
            {
                i += result.Length;
                count += result.Repeat * result.Take;
                i += result.Take - 1;
            }
            else
            {
                count++;
            }
            i++;
        }
        return count;
    }

    record struct Marker(int Take, int Repeat, int Length)
    {
        public static bool TryParse(string input, int startIndex, out Marker result)
        {
            result = default(Marker);
            if (input[startIndex] != '(')
                return false;
            int take, repeat;
            var x = input.IndexOf('x', startIndex + 1);
            if (!int.TryParse(input.Substring(startIndex + 1, x - startIndex - 1), out take))
            {
                return false;
            }
            var y = input.IndexOf(')', x + 1);
            if (!int.TryParse(input.Substring(x + 1, y - x - 1), out repeat))
            {
                return false;
            }
            result = new Marker(take, repeat, y - startIndex + 1);
            return true;
        }
        public override string ToString()
        {
            return $"({Take}x{Repeat})";
        }
    }


    public static long GetDecompressedSize2(this string input, int startIndex, int length)
    {
        long count = 0;
        var i = startIndex;
        while (i < Math.Min(startIndex + length, input.Length))
        {
            if (Marker.TryParse(input, i, out Marker result))
            {
                i += result.Length;
                var decompressed = GetDecompressedSize2(input, i, result.Take);
                count += result.Repeat * decompressed;
                i += result.Take - 1;
            }
            else
            {
                count++;
            }
            i++;
        }
        return count;
    }


}