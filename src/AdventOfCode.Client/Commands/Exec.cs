using BenchmarkDotNet.Running;

using Spectre.Console.Cli;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Run the code for a specific puzzle.")]
class Exec : Spectre.Console.Cli.AsyncCommand<Exec.Settings>
{
    AoCManager manager;

    public Exec(AoCManager manager)
    {
        this.manager = manager;
    }

    public class Settings : AoCSettings
    {
        [Description("The fully qualified name of the type containing the code for this puzzle. " +
        "Use a format string with {0} and {1} as placeholders for year and day. " +
        "(default: AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00})")]
        [CommandOption("-t|--typename")]
        public string? typeName { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {

        (var year, var day, var typeName) = (
              options.year??DateTime.Now.Year
            , options.day??DateTime.Now.Day
            , string.IsNullOrEmpty(options.typeName) ? "AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00}" : options.typeName
            );

        Console.WriteLine($"{year}, day {day}");

        DayResult result = await manager.Run(typeName, year, day);

        Console.WriteLine($"{result.part1.Value} - {result.part1.Elapsed}");
        Console.WriteLine($"{result.part2.Value} - {result.part2.Elapsed}");

        return 0;
    }
}


