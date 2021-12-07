using BenchmarkDotNet.Running;

using System.ComponentModel;

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

    public async Task Run(Options options)
    {

        (var year, var day, var typeName) = (
              options.year??DateTime.Now.Year
            , options.day??DateTime.Now.Day
            , string.IsNullOrEmpty(options.typeName) ? "AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00}" : options.typeName
            );

        Console.WriteLine($"{year}, day {day}");

        DayResult result = await AoCRunner.Run(typeName, year, day);

        Console.WriteLine(result);
    }
}


