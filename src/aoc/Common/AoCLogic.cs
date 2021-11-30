
namespace AdventOfCode.Common;
using NodaTime;

static class AoCLogic
{
    internal static int MaxDay(int year, IClock clock)
    {
        var now = clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["EST"]);

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

    internal static DirectoryInfo GetDirectory(int year, int day) => new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}"));
    internal static FileInfo GetFile(int year, int day, string fileName) => new FileInfo(Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}", fileName));
    internal static string GetFileName(int year, int day, string fileName) => Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}", fileName);

    public static IClock Clock = SystemClock.Instance;
    internal static bool IsValidAndUnlocked(int year, int day)
    {
        var now = Clock.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb["EST"]);

        // no puzzles before 2015, nor in the future
        if (year < 2015) return false;
        if (year > now.Year) return false;

        // past years: day must be between 1 and 25
        if (year < now.Year) return day switch { >= 1 and <= 25 => true, _ => false }; 

        // current year: no puzzle's if we're not in december yet
        if (12 > now.Month) return false;

        // current year, december
        return day >= 1 && day <= 25;
    }
}