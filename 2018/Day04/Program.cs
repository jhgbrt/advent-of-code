using static AoC;
using static System.Char;
using static GuardAction.Type;
using System.Globalization;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Part1(input));
    internal static Result Part2() => Run(() => Part2(input));

    public static int Part1(string[] input)
    {
        var guard = Helper.GetMostSleepingGuard(input);
        var guardId = guard.Key;
        var mostSleepingMinute = guard.GetMostSleepingMinute();
        return guardId * mostSleepingMinute;
    }

    public static int Part2(string[] input)
    {
        var guards = Parser.ToGuards(input);

        var query = (
            from g in guards
            from minute in g.GetSleepingMinutes()
            select (g.Key, minute) into x
            group x by x into g
            orderby g.Count() descending
            select g.First()
        ).ToList();

        var result = query.First();
        return result.Key * result.minute;
    }
}

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
                case FellAsleep:
                    start = g.TimeStamp;
                    break;
                case WakesUp:
                    yield return (start, g.TimeStamp);
                    break;
            }
        }
    }
}

internal struct GuardAction
{
    public enum Type
    {
        StartShift = 'G',
        FellAsleep = 'f',
        WakesUp = 'w'
    }
    public readonly DateTime TimeStamp;
    public readonly Type ActionType;
    public readonly int ID;
    public GuardAction(int id, DateTime timeStamp, Type actionType)
    {
        ID = id;
        TimeStamp = timeStamp;
        ActionType = actionType;
    }
}

internal static class Parser
{
    public static IEnumerable<IGrouping<int, GuardAction>> ToGuards(this IEnumerable<string> lines)
        => from guardaction in Parser.ToGuardActions(lines)
           orderby guardaction.TimeStamp
           group guardaction by guardaction.ID;

    public static IEnumerable<GuardAction> ToGuardActions(this IEnumerable<string> lines)
    {
        int id = default;
        foreach (var item in lines.Select(s => Parse(s)).OrderBy(x => x.timestamp))
        {
            if (item.type == GuardAction.Type.StartShift)
            {
                id = item.id.Value;
            }
            yield return new GuardAction(id, item.timestamp, item.type);
        }
    }
    private static (int? id, DateTime timestamp, GuardAction.Type type) Parse(this ReadOnlySpan<char> line)
    {
        var type = (GuardAction.Type)line[19];
        var timestamp = DateTime.ParseExact(line.Slice(1, 16), "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture);
        switch (type)
        {
            case StartShift:
                var l = 0;
                while (IsDigit(line[26 + l])) l++;
                var id = int.Parse(line.Slice(26, l));
                return (id, timestamp, type);
            default:
                return (null, timestamp, type);
        }
    }


}


