using NodaTime;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a environment variable.")]
class ShowLeaderboard
{
    AoCClient client;

    public ShowLeaderboard(AoCClient client)
    {
        this.client = client;
    }
    public record Options(
        [property: Description("Year (default: current year)")] int? year, 
        [property:Description("The leaderboard ID")]int id);

    public async Task Run(Options options)
    {
        var year = options.year ?? DateTime.Now.Year;

        var leaderboard = await client.GetLeaderBoardAsync(year, options.id, false);

        if (leaderboard == null)
        {
            Console.WriteLine("Not found?!?");
            return;
        }

        var report = from m in leaderboard.Members
                     let name = m.Name
                     let score = m.LocalScore
                     let stars = m.TotalStars
                     let lastStar = m.LastStarTimeStamp
                     where lastStar > Instant.MinValue
                     let dt = lastStar.InUtc().ToDateTimeOffset().ToLocalTime()
                     orderby score descending
                     select (name, score, stars, dt);


        Console.WriteLine(string.Join(Environment.NewLine, report.Select(x => $"{x.name,20}{x.score,5} {x.stars,2} {x.dt.TimeOfDay}")));

        //var report2 = from m in leaderboard.Members
        //              from x in m.Stars
        //              let day = x.Key
        //              let star = x.Value
        //              select (m.Name, star.Day, first: star.FirstStar.HasValue ? star.FirstStar.Value.InUtc().ToDateTimeOffset() : (DateTimeOffset?)null, second: star.SecondStar.HasValue ? star.SecondStar.Value.InUtc().ToDateTimeOffset() : (DateTimeOffset?)null);

        //Console.WriteLine(string.Join(Environment.NewLine, report2));
    }
}