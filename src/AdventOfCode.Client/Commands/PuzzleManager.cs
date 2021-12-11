using NodaTime;

using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;
class AoCRunner
{
    internal async Task<DayResult> Run(string typeName, int year, int day, Action<int, Result> progress)
    {
        dynamic aoc = GetAoC(typeName, year, day);
        var t1 = Run(() => aoc.Part1()).ContinueWith(t =>
        {
            progress(1, t.Result);
            return t.Result;
        });
        var t2 = Run(() => aoc.Part2()).ContinueWith(t =>
        {
            progress(2, t.Result);
            return t.Result;
        });
        await Task.WhenAll(t1, t2);
        var result = new DayResult(year, day, await t1, await t2);

        await Cache.WriteToCache(year, day, "result.json", JsonSerializer.Serialize(result));

        return result;
    }

    private static dynamic GetAoC(string typeName, int year, int day)
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null) throw new Exception("no entry assembly?");
        var type = assembly.GetType(string.Format(typeName, year, day));
        if (type is null) throw new InvalidOperationException($"Could not find type {typeName} for {year}, {day}. Use --typeName to override.");
        dynamic? aoc = Activator.CreateInstance(type);
        if (aoc is null) throw new InvalidOperationException($"Could not instantiate type {typeName}");
        return aoc;
    }

    static async Task<Result> Run(Func<object> f)
    {
        var sw = Stopwatch.StartNew();
        var result = await Task.Run(() => f());
        return result is -1 ? Result.Empty : new Result(ResultStatus.Unknown, result.ToString() ?? string.Empty, sw.Elapsed);
    }

}


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

    internal async Task<(HttpStatusCode status, string content)> Post(int year, int day, int part, string value)
    {
        var result = await client.PostAnswerAsync(year, day, part, value);
        await SyncAnswers(year, day);
        return result;
    }

    internal async Task<IEnumerable<LeaderboardEntry>> GetLeaderboardAsync(int year, int id)
    {
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


