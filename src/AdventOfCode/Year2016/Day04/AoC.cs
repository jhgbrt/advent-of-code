namespace AdventOfCode.Year2016.Day04;

public class AoC201604
{
    public static string[] input = Read.InputLines();

    public object Part1() => (from line in input
                                                 let room = Room.Parse(line)
                                                 where room.IsValid()
                                                 select room).Sum(r => r.SectorId);
    public object Part2() => (from line in input
                                                 let room = Room.Parse(line)
                                                 let name = room.Name
                                                 where name.Contains("northpole")
                                                 select room.SectorId).Single();
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

static class Extensions
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