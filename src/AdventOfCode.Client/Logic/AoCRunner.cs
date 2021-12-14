﻿
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace AdventOfCode.Client.Logic;

class AoCRunner
{
    internal async Task<DayResult> Run(string? typeName, int year, int day, Action<int, Result> progress)
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

    private static dynamic GetAoC(string? typeName, int year, int day)
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null) throw new Exception("no entry assembly?");

        var type = (!string.IsNullOrEmpty(typeName)
            ? assembly.GetType(string.Format(typeName, year, day))
            : null
            ) ?? (
                from t in assembly.GetTypes()
                let name = t.FullName ?? t.Name
                where name.Contains($"{year}") && name.Replace($"{year}", "").Contains($"{day:00}")
                select t
                ).FirstOrDefault();

        if (type is null) 
            throw new InvalidOperationException($"Could not find the type containing the code for {year}, {day}. Make sure the fully qualified typename contains the year and day (YYYYDD format), or use --typeName to override the convention.");

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


