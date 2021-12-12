
using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a environment variable.")]
class Leaderboard : AsyncCommand<Leaderboard.Settings>
{
    PuzzleManager manager;

    public Leaderboard(PuzzleManager manager)
    {
        this.manager = manager;
    }
    public class Settings : CommandSettings
    {
        [Description("Year (default: current year)")]
        [CommandArgument(0, "<YEAR>")]
        public int? year { get; set; }
        [CommandArgument(1, "<LEADERBOARD_ID>")]
        public int id { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        var year = options.year ?? DateTime.Now.Year;

        IEnumerable<LeaderboardEntry> entries = await manager.GetLeaderboardAsync(year, options.id);


        var table = new Table();
        table.AddColumns("rank", "member", "points", "stars", "lastStar");

        int n = 1;
        foreach (var line in entries)
        {
            table.AddRow(
                n.ToString(),
                line.name,
                line.score.ToString(),
                line.stars.ToString(),
                line.lastStar.TimeOfDay.ToString() ?? string.Empty
                );
            n++;
        }
        AnsiConsole.Write(table);

        return 0;
    }
}

