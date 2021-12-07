using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;

static class AoCRunner
{
    public static async Task<DayResult> Run(string typeName, int year, int day)
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
        var dayResult = new DayResult(year, day, await t1, await t2);

        await File.WriteAllTextAsync(Path.Combine(".cache", $"{year}-{day:00}-result.json"), JsonSerializer.Serialize(dayResult));

        return dayResult;
    }
    static async Task<Result> Run(Func<object> f)
    {
        var sw = Stopwatch.StartNew();
        var result = await Task.Run(() => f());
        return result is -1 ? Result.Empty : new Result(ResultStatus.Unknown, result.ToString()??string.Empty, sw.Elapsed);
    }
}


