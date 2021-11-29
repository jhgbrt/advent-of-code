using static AdventOfCode.Year2018.Day04.AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

namespace AdventOfCode.Year2018.Day04
{
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


