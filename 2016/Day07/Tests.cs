using System.Collections.Generic;
using System.Linq;
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

        enum State
        {
            Accumulating
        }


        [Theory]
        [InlineData("abba[mnop]qrst", true)]
        [InlineData("abcd[bddb]xyyx", false)]
        [InlineData("aaaa[qwer]tyui", false)]
        [InlineData("ioxxoj[asdfgh]zxcvbn", true)]
        [InlineData("ioxxoj[asdfgh]zxcvbnioxxoj[asdfgh]zxcvbn", true)]
        [InlineData("ioxxoj[asdfgh]zxcvbnioxxoj[asabba]zxcvbn", false)]
        public void SpecsTLS(string input, bool expected)
        {
            Assert.Equal(expected, new IPAddress(input).SupportsTLS());
        }



        [Theory]
        [InlineData("aba[bab]xyz", true)]
        [InlineData("xyx[xyx]xyx", false)]
        [InlineData("aaa[kek]eke", true)]
        [InlineData("aaa[aaa]eke", false)]
        [InlineData("zazbz[bzb]cdb", true)]
        public void SpecsSSL(string input, bool expected)
        {
            Assert.Equal(expected, new IPAddress(input).SupportsSSL());
        }


    }

    struct IPAddress
    {
        string input;

        public IPAddress(string input)
        {
            this.input = input;
        }

        enum WhereAmI
        {
            Outside,
            Inside
        }
        public bool SupportsSSL()
        {
            var whereami = WhereAmI.Outside;
            var set1 = new HashSet<string>();
            var set2 = new HashSet<string>();
            bool atLeastOnePalindrome = false;
            var startIndex = 0;
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                switch (whereami)
                {
                    case WhereAmI.Outside when c == '[':
                        whereami = WhereAmI.Inside;
                        startIndex = i + 1;
                        break;
                    case WhereAmI.Outside when i >= startIndex + 2:
                        if (IsAba(i))
                        {
                            var bab = new string(new[] { input[i - 1], input[i], input[i - 1] });
                            if (set2.Contains(bab)) return true;
                            set1.Add(input.Substring(i - 2, 3));
                        }
                        break;
                    case WhereAmI.Inside when c == ']':
                        whereami = WhereAmI.Outside;
                        startIndex = i + 1;
                        break;
                    case WhereAmI.Inside when i >= startIndex + 2:
                        if (IsAba(i))
                        {
                            var bab = new string(new[] { input[i - 1], input[i], input[i - 1] });
                            if (set1.Contains(bab)) return true;
                            set2.Add(input.Substring(i - 2, 3));
                        }
                        break;
                }
            }
            return atLeastOnePalindrome;
        }

        private bool IsAba(int i)
        {
            return input[i] == input[i - 2] && input[i - 1] != input[i];
        }

        public bool SupportsTLS()
        {
            var whereami = WhereAmI.Outside;

            bool atLeastOnePalindrome = false;
            var startIndex = 0;
            for (int i = 0; i < input.Length; i++)
            {
                var c = input[i];
                switch (whereami)
                {
                    case WhereAmI.Outside when c == '[':
                        whereami = WhereAmI.Inside;
                        startIndex = i + 1;
                        break;
                    case WhereAmI.Outside when !atLeastOnePalindrome && i >= startIndex + 3:
                        atLeastOnePalindrome = IsPalindrome(i);
                        break;
                    case WhereAmI.Inside when c == ']':
                        whereami = WhereAmI.Outside;
                        startIndex = i + 1;
                        break;
                    case WhereAmI.Inside when i >= startIndex + 3:
                        if (IsPalindrome(i)) return false;
                        break;
                }
            }
            return atLeastOnePalindrome;
        }

        private bool IsPalindrome(int i)
        {
            return (
                input[i] == input[i - 3] && input[i - 1] == input[i - 2] && input[i - 1] != input[i]
                );
        }
    }
}
