﻿
using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Run the code for a specific puzzle.")]
class Run : AsyncCommand<Run.Settings>
{
    private readonly IAoCRunner manager;

    public Run(IAoCRunner manager)
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

        DayResult result = await manager.Run(typeName, year, day, 
            (part, result) => AnsiConsole.MarkupLine($"part {part}: {result.Value} ({result.Elapsed})")
            );

        return 0;
    }
}


