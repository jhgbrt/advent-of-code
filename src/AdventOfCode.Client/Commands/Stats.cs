
using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a environment variable.")]
class Stats : AsyncCommand<Stats.Settings>
{
    private readonly IReportManager manager;

    public Stats(IReportManager manager)
    {
        this.manager = manager;
    }
    public class Settings : CommandSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {

        await foreach (var (year,m) in manager.GetMemberStats())
        {
            AnsiConsole.WriteLine($"{year}: {m.stars}, {m.score}");

        }

        return 0;
    }
}
