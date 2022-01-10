namespace Net.Code.AdventOfCode.Tool.Core;
using NodaTime;
static class AoCLogic
{

    public static IClock Clock = SystemClock.Instance;
    static ZonedDateTime Now = Clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["EST"]);

    internal static IEnumerable<(int year, int day)> Puzzles()
        => from year in Years() from day in Days(year) select (year, day);

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
        return day >= 1 && day <= now.Day && day <= 25;
    }

    internal static IEnumerable<int> Years()
    {
        for (int year = 2015; year <= Now.Year; year++)
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
        if (y != Now.Year) return false;
        if (d != Now.Day) return false;
        return IsValidAndUnlocked(y, d);
    }
}