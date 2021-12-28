using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable.")]
class Sync : AsyncCommand<Sync.Settings>
{
    private readonly PuzzleManager manager;

    public Sync(PuzzleManager manager)
    {
        this.manager = manager;
    }
    public class Settings : AoCSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day) = (options.year, options.day);
        AnsiConsole.Write($"Retrieving answers for puzzle {year}-{day:00}...");
        await manager.Sync(year, day);
        return 0;
    }
   
}
