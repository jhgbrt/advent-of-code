namespace AdventOfCode.Client;

using System.Text.Json;

using AdventOfCode.Common;

using Newtonsoft.Json.Linq;

using NodaTime;
using NodaTime.TimeZones;

using Xunit;
using Xunit.Abstractions;

public class Specs
{
    class TestClock : IClock
    {
        Instant _instant;
        public TestClock(Instant instant)
        {
            _instant = instant;
        }
        public Instant GetCurrentInstant() => _instant;
    }

    ITestOutputHelper output;
    public Specs(ITestOutputHelper output)
    {
        this.output = output;
    }

    [Theory]
    [InlineData(2021, 1, 1, 0, 0, 25)]
    [InlineData(2020, 12, 26, 0, 0, 25)]
    [InlineData(2020, 12, 25, 0, 1, 25)]
    [InlineData(2020, 12, 25, 0, 0, 25)]
    [InlineData(2020, 12, 24, 23, 59, 24)]
    [InlineData(2020, 12, 20, 0, 0, 20)]
    [InlineData(2020, 12, 1, 0, 0, 1)]
    [InlineData(2020, 11, 30, 23, 59, 0)]
    [InlineData(2020, 1, 1, 0, 0, 0)]
    public void Test(int year, int month, int day, int hour, int minute, int expected)
    {
        var timezone = DateTimeZoneProviders.Tzdb["EST"];
        var clock = new TestClock(new LocalDateTime(year, month, day, hour, minute, CalendarSystem.Iso).InZone(timezone, Resolvers.StrictResolver).ToInstant());
        var maxdays = AoCLogic.MaxDay(2020, clock);
        Assert.Equal(expected, maxdays);
    }

    IEnumerable<Member> GetMembers(JsonElement element)
    {
        foreach (var item in element.EnumerateObject())
        {
            var member = item.Value;
            //output.WriteLine(member.ToString());
            string name = string.Empty;
            int id = 0;
            int stars = 0, localScore = 0, globalScore = 0;
            Instant lastStarInstant = Instant.MinValue;
            Dictionary<int, DailyStars> completions = new Dictionary<int, DailyStars>();
            foreach (var property in member.EnumerateObject())
            {
                //output.WriteLine((property.Name, property.Value.ValueKind, property.Value).ToString());
                switch (property.Name)
                {
                    case "name": name = property.Value.GetString()!; break;
                    case "id": id = int.Parse(property.Value.GetString()!); break;
                    case "stars" when property.Value.ValueKind == JsonValueKind.Number: stars = property.Value.GetInt32(); break;
                    case "local_score" when property.Value.ValueKind == JsonValueKind.Number: localScore = property.Value.GetInt32(); break;
                    case "global_score" when property.Value.ValueKind == JsonValueKind.Number: globalScore = property.Value.GetInt32(); break;
                    case "last_star_ts" when property.Value.ValueKind == JsonValueKind.Number: lastStarInstant = Instant.FromUnixTimeSeconds(property.Value.GetInt32()); break;
                    case "last_star_ts": break;
                    case "completion_day_level":
                        completions = GetCompletions(property).ToDictionary(x => x.Day);
                        break;
                    default: throw new Exception($"unhandled property: {property.Name}");
                }
            }
            yield return new Member(id, name, stars, localScore, globalScore, lastStarInstant, completions);
        }
    }

    IEnumerable<DailyStars> GetCompletions(JsonProperty property)
    {
        foreach (var compl in property.Value.EnumerateObject())
        {
            var day = int.Parse(compl.Name);

            Instant? i1 = null;
            Instant? i2 = null;
            foreach (var star in compl.Value.EnumerateObject())
            {
                switch (int.Parse(star.Name))
                {
                    case 1: i1 = Instant.FromUnixTimeSeconds(star.Value.EnumerateObject().First().Value.GetInt32()); break;
                    case 2: i2 = Instant.FromUnixTimeSeconds(star.Value.EnumerateObject().First().Value.GetInt32()); break;
                }
            }

            yield return new DailyStars(day, i1, i2);
        }

    }

}