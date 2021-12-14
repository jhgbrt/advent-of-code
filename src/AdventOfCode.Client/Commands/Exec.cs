
using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.Diagnostics;

namespace AdventOfCode.Client.Commands;

[Description("Run the code for a specific puzzle.")]
class Exec : AsyncCommand<Exec.Settings>
{
    AoCRunner manager;

    public Exec(AoCRunner manager)
    {
        this.manager = manager;
    }

    public class Settings : AoCSettings
    {
        [Description("The fully qualified name of the type containing the code for this puzzle. " +
        "Use a format string with {0} and {1} as placeholders for year and day. " +
        "(example: MyAdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00})")]
        [CommandOption("-t|--typename")]
        public string? typeName { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {

        (var year, var day, var typeName) = (
              options.year
            , options.day
            , options.typeName
            );

        AnsiConsole.WriteLine($"{year}, day {day}");

        if (Debugger.IsAttached)
        {
            DayResult result = await manager.Run(typeName, year, day, (part, result) => AnsiConsole.MarkupLine($"part {part}: {result.Value} ({result.Elapsed})"));
        }
        else
        {
            await AnsiConsole.Status()
                .StartAsync("Running...", async ctx =>
                {
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));
                    DayResult result = await manager.Run(typeName, year, day, (part, result) => AnsiConsole.MarkupLine($"part {part}: {result.Value} ({result.Elapsed})"));
                });
        }

        return 0;
    }
}


