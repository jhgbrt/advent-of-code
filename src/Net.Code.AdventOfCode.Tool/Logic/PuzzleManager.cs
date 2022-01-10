
using Net.Code.AdventOfCode.Tool.Core;

using System.Net;
using System.Text;
using System.Text.Json;

namespace Net.Code.AdventOfCode.Tool.Logic;




class PuzzleManager : IPuzzleManager
{
    private readonly IAoCClient client;
    private readonly IAoCRunner runner;
    private readonly ICache cache;

    public PuzzleManager(IAoCClient client, IAoCRunner runner, ICache cache)
    {
        this.client = client;
        this.runner = runner;
        this.cache = cache;
    }

    public async Task<(bool status, string reason, int part)> PreparePost(int year, int day)
    {
        var puzzle = await client.GetPuzzleAsync(year, day);
        return puzzle.Status switch
        {
            Status.Locked => (false, "Puzzle is locked. Did you initialize it?", 0),
            Status.Completed => (false, "Already completed", 0),
            _ => (true, string.Empty, puzzle.Status == Status.Unlocked ? 1 : 2)
        };
    }

    public async Task Sync(int year, int day)
    {
        await client.GetPuzzleAsync(year, day, false);
    }

    public async Task<PuzzleResultStatus> GetPuzzleResult(int y, int d, bool runSlowPuzzles, string? typeName, Action<int, Result> status)
    {
        var puzzle = await client.GetPuzzleAsync(y, d);

        var result = cache.Exists(y, d, "result.json")
            ? JsonSerializer.Deserialize<DayResult>(await cache.ReadFromCache(y, d, "result.json"))
            : null;

        if (result == null || runSlowPuzzles || result.Elapsed < TimeSpan.FromSeconds(1) && !string.IsNullOrEmpty(typeName))
        {
            result = await runner.Run(typeName, y, d, status);
            return new PuzzleResultStatus(puzzle, result, true);
        }
        else
        {
            return new PuzzleResultStatus(puzzle, result, false);
        }
    }

    public async Task<(bool success, HttpStatusCode status, string content)> Post(int year, int day, int part, string value)
    {
        var (status, content) = await client.PostAnswerAsync(year, day, part, value);
        var success = content.StartsWith("That's the right answer");
        await Sync(year, day);
        if (success)
        {
            var stats = await client.GetMemberAsync(year, false);
            content = new StringBuilder(content).AppendLine().AppendLine($"You have now {stats?.TotalStars} stars and a score of {stats?.LocalScore}").ToString();
        }
        return (success, status, content);
    }


}

