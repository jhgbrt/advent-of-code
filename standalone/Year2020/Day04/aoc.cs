var sw = Stopwatch.StartNew();
var part1 = Driver.Part1();
var part2 = Driver.Part2();
Console.WriteLine((part1, part2, sw.Elapsed));
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