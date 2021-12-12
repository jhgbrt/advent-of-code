using NodaTime;

using System.Net;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;


class PuzzleManager
{
    public AoCClient client;
    public AoCRunner runner;

    public PuzzleManager(AoCClient client, AoCRunner runner)
    {
        this.client = client;
        this.runner = runner;
    }

    internal async Task<(bool status, string reason, int part)> PreparePost(int year, int day)
    {
        var puzzle = await client.GetPuzzleAsync(year, day);
        return puzzle.Status switch
        {
            Status.Locked => (false, "Puzzle is locked. Did you initialize it?", 0),
            Status.Completed => (false, "Already completed", 0),
            _ => (true, string.Empty, puzzle.Status == Status.Unlocked ? 1 : 2)
        };
    }

    internal async Task SyncAnswers(int year, int day)
    {
        var answers = FileSystem.GetFileName(year, day, "answers.json");
        var puzzle = await client.GetPuzzleAsync(year, day, false);
        var answer = puzzle.Answer;
        File.WriteAllText(answers, JsonSerializer.Serialize(answer));
    }

    internal async Task<PuzzleResultStatus> GetPuzzleResult(int y, int d, bool force, string typeName, Action<int, Result> status)
    {
        var puzzle = await client.GetPuzzleAsync(y, d);

        var result = Cache.Exists(y, d, "result.json")
            ? JsonSerializer.Deserialize<DayResult>(await Cache.ReadFromCache(y, d, "result.json"))
            : null;

        if (result == null || force || result.Elapsed < TimeSpan.FromSeconds(1) && !string.IsNullOrEmpty(typeName))
        {
            result = await runner.Run(typeName, y, d, status);
            return new PuzzleResultStatus(puzzle, result, true);
        }
        else
        {
            return new PuzzleResultStatus(puzzle, result, false);
        }
    }

    internal async Task<(bool success, HttpStatusCode status, string content)> Post(int year, int day, int part, string value)
    {
        var (status, content) = await client.PostAnswerAsync(year, day, part, value);
        var success = content.StartsWith("That's the right answer");
        await SyncAnswers(year, day);
        return (success, status, content);
    }

    internal async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int year)
    {
        var id = await client.GetLeaderBoardId(year);
        var leaderboard = await client.GetLeaderBoardAsync(year, id, false);

        if (leaderboard == null)
        {
            return Enumerable.Empty<LeaderboardEntry>();
        }

        return from m in leaderboard.Members
               let name = m.Name
               let score = m.LocalScore
               let stars = m.TotalStars
               let lastStar = m.LastStarTimeStamp
               where lastStar.HasValue && lastStar > Instant.MinValue
               let dt = lastStar.Value.InUtc().ToDateTimeOffset().ToLocalTime()
               orderby score descending
               select new LeaderboardEntry(name, score, stars, dt);
    }

    internal async IAsyncEnumerable<PuzzleResultStatus> GetPuzzleResults()
    {
        foreach (var (year, day) in AoCLogic.Puzzles())
            yield return await GetPuzzleResult(year, day, false, string.Empty, (_, _) => { });
    }

    internal async IAsyncEnumerable<PuzzleReportEntry> GetPuzzleReport()
    {
        await foreach (var p in GetPuzzleResults())
        {
            var comparisonResult = p.puzzle.Compare(p.result);
            yield return new PuzzleReportEntry(
                p.puzzle.Year,
                p.puzzle.Day,
                p.puzzle.Answer.part1,
                p.puzzle.Answer.part2,
                p.result.part1.Value,
                p.result.part1.Elapsed,
                comparisonResult.part1,
                p.result.part2.Value,
                p.result.part2.Elapsed,
                comparisonResult.part2,
                p.result.Elapsed
                );
        }

    }
}


