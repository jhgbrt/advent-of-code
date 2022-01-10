namespace AdventOfCode.Year2020.Day04
{
    public class AoC202004
    {
        public object Part1() => Driver.Part1();
        public object Part2() => Driver.Part2();
    }

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
