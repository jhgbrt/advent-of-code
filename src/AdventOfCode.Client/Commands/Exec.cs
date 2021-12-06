using BenchmarkDotNet.Running;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;

namespace AdventOfCode.Client.Commands;

[Description("Run the code for a specific puzzle.")]
class Exec : ICommand<Exec.Options>
{
    public record Options(
        [property: Description("Year (default: current year)")] int? year,
        [property: Description("Day (default: current day)")] int? day
        ) : IOptions;

    public Task Run(Options options)
    {

        (var year, var day) = (options.year??DateTime.Now.Year, options.day??DateTime.Now.Day);

        Console.WriteLine($"{year}, day {day}");

        string typeName = $"AdventOfCode.Year{year}.Day{day:00}.AoC{year}{day:00}";
        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null) throw new Exception("no entry assembly?");
        var type = assembly.GetType(typeName);

        if (type is null)
        {
            Console.WriteLine($"No implementation found for {year}, {day}");
            return Task.CompletedTask;
        }

        dynamic aoc = Activator.CreateInstance(type)!;

        Console.WriteLine($"Part 1: {Run(() => aoc.Part1())}");
        Console.WriteLine($"Part 2: {Run(() => aoc.Part2())}");

        return Task.CompletedTask;
    }
    static Result Run(Func<object> f)
    {
        var sw = Stopwatch.StartNew();
        return new(f(), sw.Elapsed);
    }
}
record Result(object Value, TimeSpan Elapsed);



