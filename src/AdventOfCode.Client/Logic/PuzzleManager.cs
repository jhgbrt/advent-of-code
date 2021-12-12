
using NodaTime;

using System.Net;
using System.Text;
using System.Text.Json;

namespace AdventOfCode.Client.Logic;


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
        var dir = new CodeFolder(year, day);
        if (!dir.Exists)
        {
            throw new Exception($"Puzzle for {year}/{day} not yet initialized. Use 'init' first.");
        }

        var puzzle = await client.GetPuzzleAsync(year, day, false);
        var answer = puzzle.Answer;
        await dir.WriteAnswers(JsonSerializer.Serialize(answer));
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
        if (success)
        {
            var stats = await client.GetMemberAsync(year);
            content = new StringBuilder(content).AppendLine().AppendLine($"You have now {stats?.TotalStars} stars and a score of {stats?.LocalScore}").ToString();
        }
        return (success, status, content);
    }


}

