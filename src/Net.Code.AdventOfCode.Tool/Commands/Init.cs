namespace Net.Code.AdventOfCode.Tool.Commands;

using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

[Description("Initialize the code for a specific puzzle. Requires AOC_SESSION set as an environment variable")]
class Init : AsyncCommand<Init.Settings>
{
    private readonly ICodeManager manager;

    public Init(ICodeManager manager)
    {
        this.manager = manager;
    }
    public class Settings : AoCSettings
    {
        [property: Description("Force (if true, refresh cache)")]
        [CommandOption("-f|--force")]
        public bool? force { get; set; }
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day, var force) = (options.year, options.day, options.force ?? false);
        await PuzzleCommandHelper.RunSinglePuzzle(year, day, async (year, day) =>
        {
            AnsiConsole.WriteLine("Puzzle is unlocked");
            await manager.InitializeCodeAsync(year, day, force, AnsiConsole.WriteLine);
        });
        return 0;
    }



}


