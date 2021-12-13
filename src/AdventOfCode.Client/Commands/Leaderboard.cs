
using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Show some stats from the configured private leaderboard. Set AOC_LEADERBOARD_ID as a environment variable.")]
class Leaderboard : AsyncCommand<Leaderboard.Settings>
{
    ReportManager manager;

    public Leaderboard(ReportManager manager)
    {
        this.manager = manager;
    }
    public class Settings : CommandSettings
    {
        [Description("Year (default: current year)")]
        [CommandArgument(0, "[YEAR]")]
        public int year { get; set; } = DateTime.Now.Year;
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        var year = options.year;

        int id;
        var ids = await manager.GetLeaderboardIds();
        if (ids.Skip(1).Any())
        {
            id = AnsiConsole.Prompt(new SelectionPrompt<(int id, string description)>().Title("Which leaderboard?").AddChoices(ids.Select(x => (x.id, x.description.EscapeMarkup())))).id;
        }
        else if (ids.Any())
        {
            id = ids.Last().id;
        }
        else
        {
            throw new Exception("no leaderboards found");
        }

        IEnumerable<LeaderboardEntry> entries = await manager.GetLeaderboardAsync(year, id);

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

