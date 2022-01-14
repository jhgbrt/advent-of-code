namespace Net.Code.AdventOfCode.Tool.Commands;

using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

[Description("Initialize the code for a specific puzzle. Requires AOC_SESSION set as an environment variable")]
class Init : SinglePuzzleCommand<Init.Settings>
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
    public override async Task ExecuteAsync(int year, int day, Settings options)
    {
        var force = options.force ?? false;
        AnsiConsole.WriteLine("Puzzle is unlocked");
        await manager.InitializeCodeAsync(year, day, force, AnsiConsole.WriteLine);
    }



}


