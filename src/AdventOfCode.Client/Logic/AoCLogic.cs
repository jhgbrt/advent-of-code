﻿namespace AdventOfCode.Client.Logic;
using NodaTime;
static class AoCLogic
{

    public static IClock Clock = SystemClock.Instance;
    static ZonedDateTime Now = Clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["EST"]);

    internal static int MaxDay(int year, IClock clock)
    {
        var now = Now;

        // for past years, all 25 puzzles are available
        if (year < now.Year) return 25;

        // for future years, no puzzles are available
        if (year > now.Year) return 0;

        return now.Month switch
        {
            // before december, nothing is available
            >= 1 and < 12 => 0,
            // between 1 - 25 december, puzzles are unlocked at midnight EST, so the nof available puzzles = day of month with max of 25
            12 => Math.Min(now.Day, 25),
            // invalid month, impossible
            _ => throw new Exception("invalid month")
        };
    }

    internal static IEnumerable<(int year, int day)> Puzzles()
    {
        var now = Now;
        for (int year = 2015; year <= now.Year; year++)
            for (int day = 1; year < now.Year && day <= 25 || now.Month == 12 && day <= now.Day; day++)
                yield return (year, day);
    }

    internal static bool IsValidAndUnlocked(int year, int day)
    {
        var now = Now;

        // no puzzles before 2015, nor in the future
        if (year < 2015) return false;
        if (year > now.Year) return false;

        // past years: day must be between 1 and 25
        if (year < now.Year) return day switch { >= 1 and <= 25 => true, _ => false };

        // current year: no puzzle's if we're not in december yet
        if (12 > now.Month) return false;

        // current year, december
        return day >= 1 && day <= now.Day;
    }

    internal static IEnumerable<int> Years()
    {
        for (int year = 2015; year <= DateTime.Now.Year; year++)
            yield return year;
    }
    internal static IEnumerable<int> Days(int year)
    {
        var now = Now;
        for (int day = 1; year < now.Year && day <= 25 || now.Month == 12 && day <= now.Day; day++)
            yield return day;
    }

    internal static bool IsToday(int y, int d)
    {
        if (y != DateTime.Now.Year) return false;
        if (d != DateTime.Now.Day) return false;
        return IsValidAndUnlocked(y, d);
    }
}