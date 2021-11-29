using static System.Convert;
using static AdventOfCode.Year2020.Day05.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2020.Day05
{
    partial class AoC
    {
        static List<Seat> seats = File.ReadLines("input.txt").Select(Seat.Parse).OrderBy(s => s.SeatID).ToList();
        internal static Result Part1() => Run(() => seats.Max(s => s.SeatID));
        internal static Result Part2() => Run(() =>
        {
            var missing =
                from item in seats.Zip(seats.Skip(1))
                where item.First.SeatID + 1 != item.Second.SeatID
                select item.First.SeatID + 1;
            return missing.Single();
        });
    }
}

record Seat(int row, int col)
{
    internal int SeatID => row * 8 + col;
    internal static Seat Parse(string input)
    {
        var binaryString = new StringBuilder(input).Replace('F', '0').Replace('B', '1').Replace('L', '0').Replace('R', '1').ToString();
        return new Seat(ToInt16(binaryString.Substring(0, 7), 2), ToInt16(binaryString.Substring(7), 2));
    }
}
