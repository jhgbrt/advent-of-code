namespace AdventOfCode.Year2018.Day04;

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
public enum Type
{
    StartShift = 'G',
    FellAsleep = 'f',
    WakesUp = 'w'
}
internal record struct GuardAction(int ID, DateTime TimeStamp, GuardAction.Type ActionType)
{
    public enum Type
    {
        StartShift = 'G',
        FellAsleep = 'f',
        WakesUp = 'w'
    }
}
