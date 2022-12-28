using static System.Convert;

var seats = File.ReadAllLines("input.txt").Select(Seat.Parse).OrderBy(s => s.SeatID).ToList();
var sw = Stopwatch.StartNew();
var part1 = seats.Max(s => s.SeatID);
var part2 = Part2();
Console.WriteLine((part1, part2, sw.Elapsed));
object Part2()
{
    var missing =
        from item in seats.Zip(seats.Skip(1))
        where item.First.SeatID + 1 != item.Second.SeatID
        select item.First.SeatID + 1;
    return missing.Single();
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