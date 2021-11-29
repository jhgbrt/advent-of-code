
using static AdventOfCode.Year2020.Day04.AoC;
Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day04
{
    partial class AoC
    {
        internal static Result Part1() => Run(() => Driver.Part1("input.txt"));
        internal static Result Part2() => Run(() => Driver.Part2("input.txt"));
    }
}

namespace AdventOfCode
{

    record Amount(int Value, string Unit)
    {
        static readonly Regex AmountRegex = new(@"^(?<Value>\d+)(?<Unit>[a-z]+)$");
        public static bool TryParse(string input, out Amount? result)
        {
            var match = AmountRegex.Match(input);
            if (!match.Success)
            {
                result = default;
                return false;
            }
            result = new(int.Parse(match.Groups["Value"].Value), match.Groups["Unit"].Value);
            return true;
        }
    }
}
