using static System.Convert;

namespace AdventOfCode.Year2020.Day05;

public class AoCImpl : AoCBase
{
    static List<Seat> seats = Read.InputLines(typeof(AoCImpl)).Select(Seat.Parse).OrderBy(s => s.SeatID).ToList();
    public override object Part1() => seats.Max(s => s.SeatID);
    public override object Part2()
    {
        var missing =
            from item in seats.Zip(seats.Skip(1))
            where item.First.SeatID + 1 != item.Second.SeatID
            select item.First.SeatID + 1;
        return missing.Single();
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
