﻿using Net.Code.AdventOfCode.Tool.Core;

using NodaTime;

using NSubstitute;

namespace Net.Code.AdventOfCode.Tool.UnitTests;

static class TestClock
{
    public static void SetClock(int year, int month, int day, int hour, int min, int sec)
    {
        var localdate = new LocalDateTime(year, month, day, hour, min, sec);
        var instant = localdate.InZoneLeniently(DateTimeZoneProviders.Tzdb["EST"]).ToInstant();
        var clock = Substitute.For<IClock>();
        clock.GetCurrentInstant().Returns(instant);
        AoCLogic.Clock = clock;
    }

}
