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
}