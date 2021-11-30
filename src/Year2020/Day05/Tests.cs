namespace AdventOfCode.Year2020.Day05;

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