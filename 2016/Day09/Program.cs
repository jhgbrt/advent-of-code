
using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    public static string input = File.ReadLines("input.txt").First();

    internal static Result Part1() => Run(() => input.GetDecompressedSize(0));
    internal static Result Part2() => Run(() => input.GetDecompressedSize2(0, input.Length));

    static Result Run<T>(int part, Func<T> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        return new(result, sw.Elapsed);
    }
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(99145L, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(10943094568L, Part2().Value);

    [Theory]
    [InlineData("ADVENT", "ADVENT")]
    [InlineData("A(1x5)BC", "ABBBBBC")]
    [InlineData("(3x3)XYZ", "XYZXYZXYZ")]
    [InlineData("(6x1)(1x3)A", "(1x3)A")]
    [InlineData("X(8x2)(3x3)ABCY", "X(3x3)ABC(3x3)ABCY")]
    [InlineData("ADVENTA(1x5)BC(3x3)XYZA(2x2)BCD(2x2)EFG(6x1)(1x3)AX(8x2)(3x3)ABCY",
        "ADVENTABBBBBCXYZXYZXYZABCBCDEFEFG(1x3)AX(3x3)ABC(3x3)ABCY")]
    public void DecompressedLength(string input, string expected)
    {
        var result2 = input.GetDecompressedSize(0);
        Assert.Equal(expected.LongCount(), result2);
    }
    [Theory]
    [InlineData("ADVENT", 6)]
    [InlineData("A(1x5)BC", 7)]
    [InlineData("X(8x2)(3x3)ABCY", 20)]
    [InlineData("(27x12)(20x12)(13x14)(7x10)(1x12)A", 241920)]
    [InlineData("(25x3)(3x3)ABC(2x3)XY(5x2)PQRSTX(18x9)(3x2)TWO(5x7)SEVEN", 445)]
    public void DecompressedLength2(string input, long expected)
    {
        var result2 = input.GetDecompressedSize2(0, input.Length);
        Assert.Equal(expected, result2);
    }
}



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
