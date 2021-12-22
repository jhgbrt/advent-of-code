
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace AdventOfCode.Client.Logic;

class AoCRunner
{
    internal async Task<DayResult> Run(string? typeName, int year, int day, Action<int, Result> progress)
    {
        dynamic? aoc = GetAoC(typeName, year, day);

        if (aoc == null) return DayResult.NotImplemented(year, day);

        var t1 = Run(() => aoc.Part1());
        progress(1, t1);

        var t2 = Run(() => aoc.Part2());
        progress(2, t2);

        var result = new DayResult(year, day, t1, t2);

        await Cache.WriteToCache(year, day, "result.json", JsonSerializer.Serialize(result));

        return result;
    }

    private static dynamic? GetAoC(string? typeName, int year, int day)
    {
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null) throw new Exception("no entry assembly?");

        Type? type = string.IsNullOrEmpty(typeName)
            ? (
                from t in assembly.GetTypes()
                let name = t.FullName ?? t.Name
                where name.Contains($"{year}") && name.Replace($"{year}", "").Contains($"{day:00}")
                select t
                ).FirstOrDefault()
            : assembly.GetType(string.Format(typeName, year, day));
        if (type is null)
            return null;

        dynamic? aoc = Activator.CreateInstance(type);

        return aoc;
    }

    static Result Run(Func<object> f)
    {
        var sw = Stopwatch.StartNew();
        var result = f();
        return result is -1 ? Result.Empty : new Result(ResultStatus.Unknown, result.ToString() ?? string.Empty, sw.Elapsed);
    }

}


