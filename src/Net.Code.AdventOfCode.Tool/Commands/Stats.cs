
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

[Description("Show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a environment variable.")]
class Stats : AsyncCommand<CommandSettings>
{
    private readonly IReportManager manager;

    public Stats(IReportManager manager)
    {
        this.manager = manager;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, CommandSettings _)
    {

        await foreach (var (year, m) in manager.GetMemberStats())
        {
            AnsiConsole.WriteLine($"{year}: {m.stars}, {m.score}");
        }

        return 0;
    }
}
