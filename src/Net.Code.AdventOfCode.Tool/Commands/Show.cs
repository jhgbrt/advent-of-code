namespace Net.Code.AdventOfCode.Tool.Commands;

using Net.Code.AdventOfCode.Tool.Core;

using System.ComponentModel;
using System.Diagnostics;

[Description("Show the puzzle instructions.")]
class Show : SinglePuzzleCommand<AoCSettings>
{
    private readonly Configuration configuration;

    public Show(Configuration configuration)
    {
        this.configuration = configuration;
    }

    public override Task ExecuteAsync(int year, int day, AoCSettings options)
    {
        ProcessStartInfo psi = new()
        {
            FileName = $"{configuration.BaseAddress}/{year}/day/{day}",
            UseShellExecute = true
        };
        Process.Start(psi);
        return Task.CompletedTask;
    }
}
