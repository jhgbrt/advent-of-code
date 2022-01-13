namespace Net.Code.AdventOfCode.Tool.Commands;

using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console.Cli;

using System.ComponentModel;
using System.Diagnostics;

[Description("Show the puzzle instructions.")]
class Show : AsyncCommand<Show.Settings>
{
    private readonly Configuration configuration;

    public Show(Configuration configuration)
    {
        this.configuration = configuration;
    }

    public class Settings : AoCSettings { }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        await PuzzleCommandHelper.RunSinglePuzzle(options, (year, day) =>
        {
            ProcessStartInfo psi = new()
            {
                FileName = $"{configuration.BaseAddress}/{year}/day/{day}",
                UseShellExecute = true
            };
            Process.Start(psi);
            return Task.CompletedTask;
        });
        return 0;
    }
}
