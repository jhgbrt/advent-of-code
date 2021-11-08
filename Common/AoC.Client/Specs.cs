
using NodaTime;
using NodaTime.TimeZones;

using Xunit;

public class Specs
{
    class TestClock : IClock
    {
        Instant _instant;
        public TestClock(Instant instant)
        {
            _instant = instant;
        }
        public Instant GetCurrentInstant() => _instant;
    }

    [Theory]
    [InlineData(2021, 1, 1, 0, 0, 25)]
    [InlineData(2020, 12, 26, 0, 0, 25)]
    [InlineData(2020, 12, 25, 0, 1, 25)]
    [InlineData(2020, 12, 25, 0, 0, 25)]
    [InlineData(2020, 12, 24, 23, 59, 24)]
    [InlineData(2020, 12, 20, 0, 0, 20)]
    [InlineData(2020, 12, 1, 0, 0, 1)]
    [InlineData(2020, 11, 30, 23, 59, 0)]
    [InlineData(2020, 1, 1, 0, 0, 0)]
    public void Test(int year, int month, int day, int hour, int minute, int expected)
    {
        var timezone = DateTimeZoneProviders.Tzdb["EST"];
        var clock = new TestClock(new LocalDateTime(year, month, day, hour, minute, CalendarSystem.Iso).InZone(timezone, Resolvers.StrictResolver).ToInstant());
        var maxdays = Program.MaxDay(2020, clock);
        Assert.Equal(expected, maxdays);

    }
}