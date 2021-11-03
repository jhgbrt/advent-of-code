using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Xunit;
using Xunit.Abstractions;

namespace Jeroen
{

    public class Tests
    {
        ITestOutputHelper output;

        public Tests(ITestOutputHelper output)
        {
            this.output = output;
        }

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

        [Fact]
        public void DoesInputHaveWhitespace()
        {
            var all = InputReader.ReadAllText();
            Assert.False(all.Contains(' '));
            Assert.False(all.Contains('\t'));
            Assert.False(all.Contains('\r'));
            Assert.False(all.Contains('\n'));
        }

        [Fact]
        public void LengthIs99145()
        {
            var input = InputReader.ReadAllText();
            Assert.Equal(99145, input.GetDecompressedSize(0));
        }

        [Fact]
        public void Part2()
        {
            var expected = 10943094568;
            var input = InputReader.ReadAllText();
            Assert.Equal(expected, input.GetDecompressedSize2(0, input.Length));

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

        struct Marker
        {
            public readonly int Take;
            public readonly int Repeat;
            public readonly int Length;

            public Marker(int take, int repeat, int length)
            {
                Take = take;
                Repeat = repeat;
                Length = length;
            }

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

}
