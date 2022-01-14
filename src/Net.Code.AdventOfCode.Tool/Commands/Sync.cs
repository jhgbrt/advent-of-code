
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;


[Description("Sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable.")]
class Sync : ManyPuzzlesCommand<AoCSettings>
{
    private readonly IPuzzleManager manager;

    public Sync(IPuzzleManager manager)
    {
        this.manager = manager;
    }

    public override async Task ExecuteAsync(int year, int day, AoCSettings _)
    {
        AnsiConsole.WriteLine($"Synchronizing for puzzle {year}-{day:00}...");
        await manager.Sync(year, day);
    }

}
