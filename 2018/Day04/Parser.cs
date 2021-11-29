using static System.Char;
using System.Globalization;
using static AdventOfCode.Year2018.Day04.GuardAction.Type;

namespace AdventOfCode.Year2018.Day04;

internal static class Parser
{
    public static IEnumerable<IGrouping<int, GuardAction>> ToGuards(this IEnumerable<string> lines)
        => from guardaction in ToGuardActions(lines)
           orderby guardaction.TimeStamp
           group guardaction by guardaction.ID;

    public static IEnumerable<GuardAction> ToGuardActions(this IEnumerable<string> lines)
    {
        int id = default;
        foreach (var item in lines.Select(s => Parse(s)).OrderBy(x => x.timestamp))
        {
            if (item.type == StartShift)
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