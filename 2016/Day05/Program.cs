using System.Security.Cryptography;

using static AdventOfCode.Year2016.Day05.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2016.Day05
{
    partial class AoC
    {
        public static string input = "ugkcyxxp";

        internal static Result Part1() => Run(() => new Cracker().GeneratePassword1(input, 8));
        internal static Result Part2() => Run(() => new Cracker().GeneratePassword2(input, 8));
    }


    public class Tests
    {
        [Fact]
        public void Test1() => Assert.Equal("d4cd2ee1", Part1().Value);
        [Fact]
        public void Test2() => Assert.Equal("f2c730e5", Part2().Value);

        [Theory]
        [InlineData(0, 0, 0, 0, true)]
        [InlineData(0, 0, 0, 0xFF, true)]
        [InlineData(0, 0, 0x0F, 0xFF, true)]
        [InlineData(0, 0, 0x1F, 0xFF, false)]
        public void ByteArrayStartsWith5ZeroesTest(byte a, byte b, byte c, byte d, bool expected)
        {
            var cracker = new Cracker();
            var result = cracker.StartsWith5Zeroes(new[] { a, b, c, d });
            Assert.Equal(expected, result);
        }

        MD5 _md5 = MD5.Create();

        [Theory]
        [InlineData("abc0", false)]
        [InlineData("abc1", false)]
        [InlineData("abc3231929", true)]
        [InlineData("abc5017308", true)]
        [InlineData("abc5278568", true)]
        public void TestHashValidation(string input, bool expected)
        {
            var cracker = new Cracker();
            var hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(input));
            var isValid = cracker.StartsWith5Zeroes(hash);
            Assert.Equal(expected, isValid);
        }

        [Fact]
        public void TestPart1()
        {
            var cracker = new Cracker();
            var password = cracker.GeneratePassword1("abc", 8);
            Assert.Equal("18f47a30", password);
        }
    }



    class Cracker
    {
        MD5 _md5 = MD5.Create();
        public bool StartsWith5Zeroes(byte[] hash) => hash[0] == 0 && hash[1] == 0 && (hash[2] & 0x0F) == hash[2];

        internal string GeneratePassword1(string input, int length)
        {
            StringBuilder sb = new StringBuilder();
            var doorid = input;
            int i = 0;
            while (true)
            {
                var s = $"{doorid}{i}";
                var hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(s));
                if (StartsWith5Zeroes(hash))
                {
                    var str = BitConverter.ToString(hash).Replace("-", "");
                    sb.Append(str[5]);
                    Console.Write(str[5]);
                }
                if (sb.Length == length)
                    break;
                i++;
            }
            Console.WriteLine();
            var password = sb.ToString();
            return password.ToLower();
        }
        internal string GeneratePassword2(string input, int length)
        {
            var sb = Enumerable.Repeat('_', length).ToArray();
            var doorid = input;
            int i = 0;
            while (true)
            {
                var s = $"{doorid}{i}";
                var hash = _md5.ComputeHash(Encoding.ASCII.GetBytes(s));
                if (StartsWith5Zeroes(hash))
                {
                    var str = BitConverter.ToString(hash).Replace("-", "");
                    if (
                        int.TryParse(str.Substring(5, 1), out int position)
                        && position >= 0 && position < 8 && sb[position] == '_'
                        )
                    {
                        sb[position] = str[6];
                        Console.WriteLine(new String(sb));
                    }
                }
                if (sb.All(c => c != '_'))
                    break;
                i++;
            }
            var password = new string(sb);
            return password.ToLower();
        }
    }
}