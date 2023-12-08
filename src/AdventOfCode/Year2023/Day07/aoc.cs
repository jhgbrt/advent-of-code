namespace AdventOfCode.Year2023.Day07;

using static HandType;

public class AoC202307
{
    static string[] input = Read.InputLines();
    public object Part1() => input.Select(s => Bid.Parse(s, false)).OrderBy(x => x).Select((b, index) => (long)b.Amount * (index + 1)).Sum();

    public object Part2() => input.Select(s => Bid.Parse(s, true)).OrderBy(x => x).Select((b, index) => (long)b.Amount * (index + 1)).Sum();
}

static partial class Regexes
{
    [GeneratedRegex(@"(?<cards>[^ ]{5}) (?<bid>\d+)")]
    public static partial Regex BidRegex();
    public static (Card[], int) ParseBid(string s, bool wildcard)
    {
        var match = BidRegex().Match(s);
        var hand = match.Groups["cards"].Value.Select(c => Card.From(c, wildcard)).ToArray();
        var bid = int.Parse(match.Groups["bid"].Value);
        return (hand, bid);
    }
}

public record struct Card(char Name, int Value)
{
    public static Card From(char c, bool wildcard) => new(c, c switch
    {
        'A' => 14,
        'K' => 13,
        'Q' => 12,
        'J' => wildcard ? 0 : 11,
        'T' => 10,
        _ => c - '0'
    });
}

public enum HandType
{
    FiveOfAKind = 7,
    FourOfAKind = 6,
    FullHouse = 5,
    ThreeOfAKind = 4,
    TwoPair = 3,
    OnePair = 2,
    HighCard = 1
}

record Bid(Card[] Hand, int Amount, bool wildcard) : IComparable<Bid>
{
    public static Bid Parse(string s, bool wildcard)
    {
        var (hand, bid) = Regexes.ParseBid(s, wildcard);
        return new Bid(hand, bid, wildcard);
    }
    public int CompareTo(Bid? other)
    {
        if (Type > other?.Type) return 1;
        if (Type < other?.Type) return -1;
        for (int i = 0; i < 5; i++)
        {
            if (Hand[i].Value > other?.Hand[i].Value) return 1;
            if (Hand[i].Value < other?.Hand[i].Value) return -1;
        }
        return 0;
    }

    public HandType Type => (wildcard, Hand.Count(c => c.Name == 'J')) switch
    {
        (true, 4 or 5) => FiveOfAKind,
        (true, 3) => CardCounts switch
        {
            [2] => FiveOfAKind,
            [1, 1] => FourOfAKind
        },
        (true, 2) => CardCounts switch
        {
            [3] => FiveOfAKind,
            [1, 2] => FourOfAKind,
            [1, 1, 1] => ThreeOfAKind,
        },
        (true, 1) => CardCounts switch
        {
            [4] => FiveOfAKind,
            [1, 3] => FourOfAKind,
            [2, 2] => FullHouse,
            [1, 1, 2] => ThreeOfAKind,
            [1, 1, 1, 1] => OnePair
        },
        _ => CardCounts switch
        {
            [5] => FiveOfAKind,
            [1, 4] => FourOfAKind,
            [2, 3] => FullHouse,
            [1, 1, 3] => ThreeOfAKind,
            [1, 2, 2] => TwoPair,
            [1, 1, 1, 2] => OnePair,
            _ => HighCard
        }
    };

    private int[] CardCounts => (
        from c in Hand
        where !wildcard || c.Name != 'J'
        group c by c into g
        let count = g.Count()
        orderby count
        select count).ToArray();

}

public class Tests
{
    [Theory]
    [InlineData("AAAAA 100", FiveOfAKind)]
    [InlineData("AAAA2 100", FourOfAKind)]
    [InlineData("A2AAA 100", FourOfAKind)]
    [InlineData("AAA22 100", FullHouse)]
    [InlineData("22AAA 100", FullHouse)]
    [InlineData("33312 100", ThreeOfAKind)]
    [InlineData("22334 100", TwoPair)]
    [InlineData("22456 100", OnePair)]
    [InlineData("23456 100", HighCard)]
    public void TestHandType(string s, HandType expected) 
    {
        var bid = Bid.Parse(s, false);
        Assert.Equal(expected, bid.Type);
    }

    [Fact]
    public void CompareHandType2()
    {
        var s1 = "JKKK2 12";
        var b1 = Bid.Parse(s1, true);
        var s2 = "QQQQ2 12";
        var b2 = Bid.Parse(s2, true);
        Assert.Equal(-1, b1.CompareTo(b2));
    }
    [Fact]
    public void TestHandType2()
    {
        var s = "JKKK2 12";
        var b2 = Bid.Parse(s, true);
        Assert.Equal(HandType.FourOfAKind, b2.Type);
    }


}

