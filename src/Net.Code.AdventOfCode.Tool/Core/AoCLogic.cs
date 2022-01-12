namespace Net.Code.AdventOfCode.Tool.Core;
using NodaTime;
static class AoCLogic
{

    public static IClock Clock = SystemClock.Instance;
    static ZonedDateTime Now => Clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["EST"]);
    public static int? Year => Now.Year;
    public static int? Day => Now.Month == 12 && Now.Day >= 1 && Now.Day <= 25 ? Now.Day : null;
    internal static IEnumerable<(int year, int day)> Puzzles()
        => from year in Years() from day in Days(year) select (year, day);
    internal static IEnumerable<(int year, int day)> Puzzles(int? year, int? day)
    {
        if (!year.HasValue && Now.Month == 12 && Now.Day <= 25)
        {
            year = Now.Year;
        }
        if (!year.HasValue && day.HasValue)
        {
            throw new ArgumentException("Outside the advent, it's meaningless to only specify a day");
        }
        if (!day.HasValue && Now.Month == 12 && Now.Day <= 25)
        {
            day = Now.Day;
        }
        return from y in Years()
               where !year.HasValue || year.Value == y
               from d in Days(y)
               where !day.HasValue || day.Value == d
               select (y, d);
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
        for (int day = 1; (year < now.Year && day <= 25) || 
                          (now.Month == 12 && day <= Math.Min(25,now.Day)); day++)
            yield return day;
    }

    internal static bool IsToday(int y, int d)
    {
        if (y != Now.Year) return false;
        if (d != Now.Day) return false;
        return IsValidAndUnlocked(y, d);
    }
}