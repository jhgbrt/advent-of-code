using System;
using System.IO;
using System.Linq;
using System.Text;
using Xunit;
using static System.Convert;

var seats = File.ReadLines("input.txt").Select(Seat.Parse).OrderBy(s => s.SeatID).ToList();

Console.WriteLine(seats.Max(s => s.SeatID));

var missing = 
    from item in seats.Zip(seats.Skip(1))
    where item.First.SeatID + 1 != item.Second.SeatID 
    select item.First.SeatID + 1;

Console.WriteLine(missing.Single());

record Seat(int row, int col) 
{ 
    internal int SeatID => row * 8 + col; 
    internal static Seat Parse(string input)
    {
        var binaryString = new StringBuilder(input).Replace('F', '0').Replace('B', '1').Replace('L', '0').Replace('R', '1').ToString();
        return new Seat(ToInt16(binaryString.Substring(0, 7), 2), ToInt16(binaryString.Substring(7), 2));
    }
}

public class Tests
{
    [Theory]
    [InlineData("FBFBBFFRLR", 44, 5, 357)]
    [InlineData("BFFFBBFRRR", 70, 7, 567)]
    [InlineData("FFFBBBFRRR", 14, 7, 119)]
    [InlineData("BBFFBBFRLL", 102, 4, 820)]
    public void TranslateBoardingPass(string input, int row, int col, int seatId)
    {
        var seat = Seat.Parse(input);
        Assert.Equal(new Seat(row, col), seat);
        Assert.Equal(seatId, seat.SeatID);
    }
}
