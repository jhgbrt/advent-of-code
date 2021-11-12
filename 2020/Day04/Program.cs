using AdventOfCode;

using Microsoft.CodeAnalysis.CSharp;

using static AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    internal static Result Part1() => Run(() => Driver.Part1("input.txt"));
    internal static Result Part2() => Run(() => Driver.Part2("input.txt"));
}


namespace AdventOfCode
{
    static class Driver
    {
        internal static long Part1(string input) => Parse(input).Count(RequiredFieldsPresent);
        internal static long Part2(string input) => Parse(input).Where(RequiredFieldsPresent).Count(IsValid);
        internal static IEnumerable<IReadOnlyDictionary<string, string>> Parse(string path, Action<string> log = null)
            => Parse(new StreamReader(File.OpenRead(path)), log);

        internal static IEnumerable<IReadOnlyDictionary<string, string>> Parse(TextReader reader, Action<string> log = null)
        {
            if (log == null) log = s => { };
            var cb = new CredentialBuilder();
            TransitionFunc Next = OnKey;
            while (reader.Peek() >= 0)
            {
                char c = (char)reader.Read();
                log(Next.Method.Name);
                log(c.ToCSharpLiteral());
                (cb, Next) = Next(cb, c);
                log(cb.ToString());
                if (cb.IsComplete) 
                    yield return cb.ToCredential();

            }
            yield return cb.RecordKeyValue().Finalize().ToCredential();
        }

        delegate (CredentialBuilder, TransitionFunc) TransitionFunc(CredentialBuilder cb, char c);

        static (CredentialBuilder, TransitionFunc) OnKey(CredentialBuilder cb, char c) => c switch
        {
            ':' => (cb, OnValue),
            _ when char.IsLetter(c) => (cb.AppendToKey(c), OnKey),
            _ => throw new($"unexpected character while parsing key: {c}")
        };

        static (CredentialBuilder, TransitionFunc) OnValue(CredentialBuilder cb, char c) => c switch
        {
            ' ' => (cb.RecordKeyValue(), OnKey),
            '\n' => (cb.RecordKeyValue(), OnFirstNewLine),
            _ => (cb.AppendToValue(c), OnValue)
        };

        static (CredentialBuilder, TransitionFunc) OnFirstNewLine(CredentialBuilder cb, char c) => c switch
        {
            '\n' => (cb.Finalize(), OnAdditionalNewLine), // second newline
            //'\n' => (cb, OnFirstNewLine),
            _ => (cb.AppendToKey(c), OnKey)
        };

        static (CredentialBuilder, TransitionFunc) OnAdditionalNewLine(CredentialBuilder cb, char c) => c switch
        {
            '\r' or '\n' => (cb, OnAdditionalNewLine),
            _ => (cb.AppendToKey(c), OnKey)
        };

        static string ToCSharpLiteral(this char c) => SymbolDisplay.FormatLiteral(c, false);

        static readonly string[] ValidKeys = new[] { "byr", "ecl", "eyr", "hcl", "hgt", "iyr", "pid" };

        internal static bool RequiredFieldsPresent(this IReadOnlyDictionary<string, string> credential) => (
            from key in credential.Keys
            join vkey in ValidKeys on key equals vkey
            select key
            ).Count() == ValidKeys.Length;

        static readonly Regex ColorRegex = new("^#[0-9a-f]{6}$", RegexOptions.Compiled);

        internal static bool IsValid(this IReadOnlyDictionary<string, string> credential)
            => credential.All(kv => IsValid(kv.Key, kv.Value));

        internal static bool IsValid(string key, string value) => key switch
        {
            "byr" => int.TryParse(value, out var y) && y >= 1920 && y <= 2002,
            "iyr" => int.TryParse(value, out var y) && y >= 2010 && y <= 2020,
            "eyr" => int.TryParse(value, out var y) && y >= 2020 && y <= 2030,
            "hgt" => Amount.TryParse(value, out var hgt) && hgt switch
            {
                { Unit: "cm", Value: >= 150 and <= 193 } => true,
                { Unit: "in", Value: >= 59 and <= 76 } => true,
                _ => false
            },
            "hcl" => ColorRegex.IsMatch(value),
            "ecl" => value switch
            {
                "amb" or "blu" or "brn" or "gry" or "grn" or "hzl" or "oth" => true,
                _ => false
            },
            "pid" => value.Length == 9 && value.All(char.IsDigit),
            _ => true
        };
    }

    class CredentialBuilder
    {
        StringBuilder key = new();
        StringBuilder value = new();
        Dictionary<string, string> dictionary = new();

        internal bool IsComplete { get; private set; }

        internal CredentialBuilder AppendToKey(char c)
        {
            key.Append(c);
            return this;
        }
        internal CredentialBuilder AppendToValue(char c)
        {
            value.Append(c);
            return this;
        }
        internal CredentialBuilder RecordKeyValue()
        {
            dictionary[key.ToString()] = value.ToString();
            key.Clear();
            value.Clear();
            return this;
        }
        internal CredentialBuilder Finalize()
        {
            IsComplete = true;
            return this;
        }
        internal IReadOnlyDictionary<string, string> ToCredential()
        {
            var result = dictionary;
            IsComplete = false;
            dictionary = new();
            return result;
        }

        public override string ToString()
        {
            return $"{string.Join(";", dictionary.Select(d => $"{d.Key}:{d.Value}"))} + {key}:{value} (complete: {IsComplete})";
        }
    }

    record Amount(int Value, string Unit)
    {
        static readonly Regex AmountRegex = new(@"^(?<Value>\d+)(?<Unit>[a-z]+)$");
        public static bool TryParse(string input, out Amount result)
        {
            var match = AmountRegex.Match(input);
            if (!match.Success)
            {
                result = null;
                return false;
            }
            result = new(int.Parse(match.Groups["Value"].Value), match.Groups["Unit"].Value);
            return true;
        }
    }

    public class Tests
    {
        readonly ITestOutputHelper _output;
        public Tests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TestPart1() => Assert.Equal(2, Driver.Part1("sample.txt"));
        [Fact]
        public void TestPart2() => Assert.Equal(2, Driver.Part2("sample.txt"));
        [Fact]
        public void TestPart2Valid() => Assert.Equal(4, Driver.Part2("valid.txt"));
        [Fact]
        public void TestPart2Invalid() => Assert.Equal(0, Driver.Part2("invalid.txt"));

        [Fact]
        public void TestParse()
        {
            var tr = new StringReader("byr:abc def:123\r\nxyz:asdf\r\n\r\nbyr:edf");
            var items = Driver.Parse(tr, s => _output.WriteLine(s)).ToList();
            Assert.Equal("asdf", items[0]["xyz"]);
            Assert.Equal("abc", items[0]["byr"]);
            Assert.Equal("123", items[0]["def"]);
            Assert.Equal("edf", items[1]["byr"]);
        }

        [Theory]
        [InlineData("", false, 0, null)]
        [InlineData("123", false, 0, null)]
        [InlineData("asb", false, 0, null)]
        [InlineData("123abc", true, 123, "abc")]
        public void AmountParse(string input, bool expected, int value, string unit)
        {
            Assert.Equal(expected, Amount.TryParse(input, out var a));
            if (expected)
            {
                Assert.Equal(value, a.Value);
                Assert.Equal(unit, a.Unit);
            }
        }

        [Theory]
        [InlineData("", false)]
        [InlineData("123", false)]
        [InlineData("asb", false)]
        [InlineData("123abc", false)]
        [InlineData("#", false)]
        [InlineData("#1234", false)]
        [InlineData("#123abc", true)]
        [InlineData("#789456", true)]
        [InlineData("#abcedf", true)]
        [InlineData("#abcedg", false)]
        [InlineData("#12345g", false)]
        public void ValidColor(string input, bool expected)
        {
            Assert.Equal(expected, Driver.IsValid("hcl", input));
        }

        [Theory]
        [InlineData("byr", "1919", false)]
        [InlineData("byr", "2003", false)]
        [InlineData("byr", "2002", true)]
        [InlineData("byr", "1920", true)]
        [InlineData("byr", "1970", true)]
        [InlineData("iyr", "1919", false)]
        [InlineData("iyr", "2009", false)]
        [InlineData("iyr", "2010", true)]
        [InlineData("iyr", "2020", true)]
        [InlineData("iyr", "2015", true)]
        [InlineData("eyr", "2019", false)]
        [InlineData("eyr", "2031", false)]
        [InlineData("eyr", "2020", true)]
        [InlineData("eyr", "2030", true)]
        [InlineData("eyr", "2025", true)]
        public void IsValidYear(string key, string value, bool expected)
        {
            Assert.Equal(expected, Driver.IsValid(key, value));
        }
    }
}
