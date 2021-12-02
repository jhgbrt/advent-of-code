using NodaTime;

namespace AdventOfCode.Client;

class ShowLeaderboard
{
    AoCClient client;
    private readonly int leaderboardID;

    public ShowLeaderboard(AoCClient client, int leaderboardID)
    {
        this.client = client;
        this.leaderboardID = leaderboardID;
    }
    public record Options(int? year);

    public async Task Run(Options options)
    {
        var year = options.year ?? DateTime.Now.Year;

        var leaderboard = await client.GetLeaderBoardAsync(year, leaderboardID, false);

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
                     let dt = lastStar.InUtc().ToDateTimeOffset()
                     orderby dt descending
                     select (name, score, stars, lastStar);

        Console.WriteLine(string.Join(Environment.NewLine, report));
    }

}