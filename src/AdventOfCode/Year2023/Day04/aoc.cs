namespace AdventOfCode.Year2023.Day04;
public class AoC202304
{
    static string[] input = Read.InputLines();
    static ImmutableArray<Card> items = input.Select(Card.Parse).ToImmutableArray();
    public double Part1() => (from c in items
                              let e = c.NofWinning
                              where e > 0
                              select Pow(2, e - 1)).Sum();
    public object Part2() 
    {
        var cards = items.ToList();
        foreach (var card in cards)
        {
            for (var c = 0; c < card.Count; c++)
            {
                foreach (var offset in Range(0, card.NofWinning))
                {
                    cards[card.Id + offset].Count++;
                }
            }
        }
        return cards.Sum(c => c.Count);
    }
}

class Card
{
    int id; int nofwinning;
    public Card(int id, int[] winning, int[] numbers)
    {
        this.id = id;
        nofwinning = winning.Count(numbers.Contains);
        Count = 1;
    }
    public int Count { get; set; }
    public int Id => id;

    public int NofWinning => nofwinning;
    public static Card Parse(string s)
    {
        var match = Regexes.MyRegex().Match(s);
        var id = int.Parse(match.Groups["id"].Value);
        var winning = match.Groups["winning"].Value.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        var numbers = match.Groups["numbers"].Value.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToArray();
        return new(id, winning, numbers);
    }
}

static partial class Regexes
{
    [GeneratedRegex(@"^Card +(?<id>\d.*): +(?<winning>[\d ]+) \| +(?<numbers>.*)")]
    public static partial Regex MyRegex();
}

public class Tests
{
    [Fact]
    public void Can_Parse_Card()
    {
        var regex = Regexes.MyRegex();
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