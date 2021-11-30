namespace AdventOfCode.Year2018.Day04;

internal static class Helper
{
    public static IGrouping<int, GuardAction> GetMostSleepingGuard(string[] input)
    {
        var result = (
            from guard in Parser.ToGuards(input)
            let minutesAsleep = (
                guard.CountMinutesAsleep()
            )
            orderby minutesAsleep descending
            select guard
            ).First();
        return result;
    }
    public static int CountMinutesAsleep(this IGrouping<int, GuardAction> guard)
        => (
            from interval in guard.GetSleepingIntervals()
            select interval.GetMinutes()
            ).Sum();

    private static int GetMinutes(this (DateTime start, DateTime end) interval)
        => (int)interval.end.Subtract(interval.start).TotalMinutes;

    public static int GetMostSleepingMinute(this IGrouping<int, GuardAction> guard)
        => (from m in guard.GetSleepingMinutes()
            group m by m into g
            orderby g.Count() descending
            select g.Key).First();

    public static IEnumerable<int> GetSleepingMinutes(this IGrouping<int, GuardAction> guard)
        => from interval in guard.GetSleepingIntervals()
           from minute in Enumerable.Range(interval.start.Minute, (int)interval.end.Subtract(interval.start).TotalMinutes)
           select minute;


    public static IEnumerable<(DateTime start, DateTime end)> GetSleepingIntervals(this IGrouping<int, GuardAction> guard)
    {
        var start = default(DateTime);
        foreach (var g in guard.OrderBy(g => g.TimeStamp))
        {
            switch (g.ActionType)
            {
                case GuardAction.Type.FellAsleep:
                    start = g.TimeStamp;
                    break;
                case GuardAction.Type.WakesUp:
                    yield return (start, g.TimeStamp);
                    break;
            }
        }
    }
}