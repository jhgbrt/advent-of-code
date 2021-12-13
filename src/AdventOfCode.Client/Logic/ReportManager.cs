
using NodaTime;

using Spectre.Console;

namespace AdventOfCode.Client.Logic;

class ReportManager
{
    AoCClient client;
    private readonly PuzzleManager manager;

    public ReportManager(AoCClient client, PuzzleManager manager)
    {
        this.client = client;
        this.manager = manager;
    }

    internal async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int year)
    {
        var id = await client.GetLeaderboardId();
        var leaderboard = await client.GetLeaderBoardAsync(year, id, false);

        if (leaderboard == null)
        {
            return Enumerable.Empty<LeaderboardEntry>();
        }

        return from m in leaderboard.Members.Values
               let name = m.Name
               let score = m.LocalScore
               let stars = m.TotalStars
               let lastStar = m.LastStarTimeStamp
               where lastStar.HasValue && lastStar > Instant.MinValue
               let dt = lastStar.Value.InUtc().ToDateTimeOffset().ToLocalTime()
               orderby score descending
               select new LeaderboardEntry(name, score, stars, dt);
    }

    internal async IAsyncEnumerable<(int year, MemberStats stats)> GetMemberStats()
    {
        foreach (var y in AoCLogic.Years())
        {
            var m = await client.GetMemberAsync(y);
            if (m == null) continue;
            yield return (y, new MemberStats(m.Name, m.TotalStars, m.LocalScore));
        }
    }

    internal async Task<MemberStats?> GetMemberStats(int year)
    {
        var m = await client.GetMemberAsync(year);
        if (m == null) return null;
        return new MemberStats(m.Name, m.TotalStars, m.LocalScore);
    }

    internal async IAsyncEnumerable<PuzzleResultStatus> GetPuzzleResults()
    {
        foreach (var (year, day) in AoCLogic.Puzzles())
            yield return await manager.GetPuzzleResult(year, day, false, string.Empty, (_, _) => { });
    }

    internal async IAsyncEnumerable<PuzzleReportEntry> GetPuzzleReport(ResultStatus? status, int? slowerthan)
    {
        await foreach (var p in GetPuzzleResults())
        {
            var comparisonResult = p.puzzle.Compare(p.result);

            if (status.HasValue && (comparisonResult.part1 != status.Value || comparisonResult.part2 != status.Value)) continue;
            if (slowerthan.HasValue && p.result.Elapsed < TimeSpan.FromSeconds(slowerthan.Value)) continue;

            yield return new PuzzleReportEntry(
                p.puzzle.Year,
                p.puzzle.Day,
                p.puzzle.Answer.part1,
                p.puzzle.Answer.part2,
                p.result.part1.Value,
                p.result.part1.Elapsed,
                comparisonResult.part1.GetDisplayName(),
                p.result.part2.Value,
                p.result.part2.Elapsed, 
                comparisonResult.part2.GetDisplayName(),
                p.result.Elapsed
                );
        }
    }
}