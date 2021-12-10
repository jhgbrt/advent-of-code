using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;

class AoCManager
{
    public AoCClient client;

    public AoCManager(AoCClient client)
    {
        this.client = client;
    }

    internal async Task<DayResult> Run(string typeName, int year, int day)
    {
        var assembly = Assembly.GetEntryAssembly();

        if (assembly == null) throw new Exception("no entry assembly?");
        var type = assembly.GetType(string.Format(typeName, year, day));
        if (type is null) throw new InvalidOperationException($"Could not find type {typeName} for {year}, {day}. Use --typeName to override.");
        dynamic? aoc = Activator.CreateInstance(type);
        if (aoc is null) throw new InvalidOperationException($"Could not instantiate type {typeName}");
        var t1 = Run(() => aoc.Part1());
        var t2 = Run(() => aoc.Part2());
        await Task.WhenAll(t1, t2);
        var result = new DayResult(year, day, await t1, await t2);

        await Cache.WriteToCache(year, day, "result.json", JsonSerializer.Serialize(result));

        return result;
    }
    static async Task<Result> Run(Func<object> f)
    {
        var sw = Stopwatch.StartNew();
        var result = await Task.Run(() => f());
        return result is -1 ? Result.Empty : new Result(ResultStatus.Unknown, result.ToString()??string.Empty, sw.Elapsed);
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

    internal async Task Sync(int year, int day)
    {
        var answers = FileSystem.GetFileName(year, day, "answers.json");
        var puzzle = await client.GetPuzzleAsync(year, day, false);
        var answer = puzzle.Answer;
        File.WriteAllText(answers, JsonSerializer.Serialize(answer));
    }

    internal async Task<(HttpStatusCode status, string content)> Post(int year, int day, int part, string value)
    {
        var result = await client.PostAnswerAsync(year, day, part, value);
        await Sync(year, day);
        return result;
    }


}


