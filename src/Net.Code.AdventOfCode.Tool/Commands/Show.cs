namespace Net.Code.AdventOfCode.Tool.Commands;

using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
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
    public override Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day) = (options.year, options.day);
        ProcessStartInfo psi = new()
        {
            FileName = $"{configuration.BaseAddress}/{year}/day/{day}",
            UseShellExecute = true
        };
        Process.Start(psi);
        return Task.FromResult(0);
    }
}
