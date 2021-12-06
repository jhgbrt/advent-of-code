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
        [property: Description("Day (default: current day)")] int? day,
        [property: Description("The fully qualified name of the type containing the code for this puzzle. " +
        "Use a format string with {0} and {1} as placeholders for year and day. " +
        "(default: AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00})")] string? typeName
        ) : IOptions;

    public Task Run(Options options)
    {

        (var year, var day, var typeName) = (
              options.year??DateTime.Now.Year
            , options.day??DateTime.Now.Day
            , string.IsNullOrEmpty(options.typeName) ? "AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00}" : options.typeName
            );

        Console.WriteLine($"{year}, day {day}");

        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null) throw new Exception("no entry assembly?");
        Console.WriteLine(string.Format(typeName, year, day));
        var type = assembly.GetType(string.Format(typeName, year, day));

        if (type is null)
        {
            Console.WriteLine($"Could not find type {typeName} for {year}, {day}. Use --typeName to override.");
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



