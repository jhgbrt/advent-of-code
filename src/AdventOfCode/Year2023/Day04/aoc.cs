namespace AdventOfCode.Year2023.Day04;
public class AoC202304
{
    static string[] input = Read.InputLines();
    static Regex regex = Regexes.CardRegex();
    static ImmutableArray<Card> items = input.Select(s => regex.As<Card>(s)).ToImmutableArray();
    public double Part1() => (from c in items
                              let e = c.NofWins
                              where e > 0
                              select Pow(2, e - 1)).Sum();
    public long Part2() 
    {
        (int nofwins, int count)[] cards = (
            from item in items
            let nofwins = item.NofWins
            select (nofwins, count: 1)).ToArray();

        for (int i = 0; i < cards.Length; i++)
        {
            var card = cards[i];
            for (var c = 0; c < card.count; c++)
            {
                foreach (var offset in Range(1, card.nofwins))
                {
                    cards[i + offset].count++;
                }
            }
        }
        return cards.Sum(c => c.count);
    }
}

record Card(int id, int[] winning, int[] numbers)
{
    public int NofWins => winning.Count(numbers.Contains);
}

static partial class Regexes
{
    [GeneratedRegex(@"^Card +(?<id>\d.*): +(?<winning>[\d ]+) \| +(?<numbers>.*)")]
    public static partial Regex CardRegex();
}

public class Tests
{
    [Fact]
    public void Can_Parse_Card()
    {
        var regex = Regexes.CardRegex();
        var input = "Card  1: 12 34 45 |  8 90 12 34 45";
        var match = regex.Match(input);
        Assert.True(match.Success);
        var id = int.Parse(match.Groups["id"].Value);
        Assert.Equal(1, id);
        
        var winning = match.Groups["winning"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries|StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
        Assert.Equal(new[] { 12, 34, 45 }, winning);

        var numbers = match.Groups["numbers"].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Select(int.Parse).ToArray();
        Assert.Equal(new[] { 8,90,12,34,45}, numbers);
    }

}