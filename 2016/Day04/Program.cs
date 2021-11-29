
using static AdventOfCode.Year2016.Day04.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2016.Day04
{
    partial class AoC
    {
        static bool test = false;
        public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

        internal static Result Part1() => Run(() => (from line in input
                                                     let room = Room.Parse(line)
                                                     where room.IsValid()
                                                     select room).Sum(r => r.SectorId));
        internal static Result Part2() => Run(() => (from line in input
                                                     let room = Room.Parse(line)
                                                     let name = room.Name
                                                     where name.Contains("northpole")
                                                     select room.SectorId).Single());
    }
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(361724, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(482, Part2().Value);
    [Fact]
    public void Can_Parse_Room()
    {
        var input = "aaaaa-bbb-z-y-x-123[abxyz]";
        var room = Room.Parse(input);
        Assert.Equal("aaaaa-bbb-z-y-x", room.Id);
        Assert.Equal("abxyz", room.Checksum);
        Assert.Equal(123, room.SectorId);
    }


    [Theory]
    [InlineData("aaaaa-bbb-z-y-x-123[abxyz]", true)]
    [InlineData("a-b-c-d-e-f-g-h-987[abcde]", true)]
    [InlineData("not-a-real-room-404[oarel]", true)]
    [InlineData("totally-real-room-200[decoy]", false)]
    public void Test(string s, bool expected)
    {
        Room room = Room.Parse(s);
        var result = room.IsValid();
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData('a', 0, 'a')]
    [InlineData('a', 1, 'b')]
    [InlineData('a', 25, 'z')] //  1 -> 26 =>  1 + 25 =  1 + 
    [InlineData('a', 26, 'a')] //  1 ->  1 =>  1 +  0 =  1 + 26 - 26
    [InlineData('b', 25, 'a')] //  2 ->  1 =>  2 -  1 =  2 + 25 - 26
    [InlineData('y', 3, 'b')]  //  'a' + (3+25)%26 = 'a' + 2 
    [InlineData('z', 1, 'a')]
    [InlineData('q', 343, 'v')]
    [InlineData('z', 343, 'e')]
    public void TestModulo26(char c, int rotations, char expected)
    {
        var offset = (c + rotations - 'a') % 26;
        var result = (char)('a' + offset);
        Assert.Equal(expected, result);
    }

    [Fact]
    public void TestPart2()
    {
        var encrypted = "qzmt-zixmtkozy-ivhz";
        var decrypted = encrypted.Decrypt(343);
        Assert.Equal("very encrypted name", decrypted);
    }
}



record Room(string Id, int SectorId, string Checksum)
{
    public static Room Parse(string s)
    {
        var regex = new Regex(@"(?<id>[a-z\-]+)-(?<sectorid>\d+)\[(?<checksum>.+)\]");
        var match = regex.Match(s);
        var id = match.Groups["id"].Value;
        var sectorId = int.Parse(match.Groups["sectorid"].Value);
        var checksum = match.Groups["checksum"].Value;
        return new Room(id, sectorId, checksum);
    }

    public bool IsValid() => Id.Where(char.IsLetter).GroupBy(c => c).OrderByDescending(g => g.Count()).ThenBy(g => g.Key).Take(5).Select(g => g.Key).SequenceEqual(Checksum);

    public string Name => Id.Decrypt(SectorId);
}

public static class Extensions
{
    public static string Decrypt(this string encrypted, int rotations)
    {
        StringBuilder sb = new StringBuilder();
        foreach (var c in encrypted)
        {
            if (c == '-') sb.Append(' ');
            else
            {
                var offset = (c + rotations - 'a') % 26;
                var result = (char)('a' + offset);
                sb.Append(result);
            }
        }
        return sb.ToString();
    }
}