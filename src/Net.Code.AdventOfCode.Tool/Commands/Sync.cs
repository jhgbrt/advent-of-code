
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

[Description("Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable.")]
class Sync : AsyncCommand<Sync.Settings>
{
    private readonly IPuzzleManager manager;

    public Sync(IPuzzleManager manager)
    {
        this.manager = manager;
    }
    public class Settings : AoCSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        await PuzzleCommandHelper.RunMultiPuzzle(options, async (year, day) =>
        {
            AnsiConsole.WriteLine($"Synchronizing for puzzle {year}-{day:00}...");
            await manager.Sync(year, day);
        });
        return 0;
    }

}
