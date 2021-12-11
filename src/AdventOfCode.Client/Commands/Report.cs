using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Show a list of all puzzles, their status (unlocked, answered), and the answers posted.")]
class Report : AsyncCommand<Report.Settings>
{
    PuzzleManager manager;
    public Report(PuzzleManager manager)
    {
        this.manager = manager;
    }
    public class Settings : CommandSettings
    {
        [property: Description("status")]
        [CommandOption("--status")]
        public ResultStatus? status { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {

        IEnumerable<PuzzleReportEntry> report = await manager.GetPuzzleReport().ToListAsync();

        if (options.status != null)
        {
            report = report.Where(x => x.status1 == options.status.Value || x.status2 == options.status.Value);
        }

        var table = new Table();
        table.AddColumns(typeof(PuzzleReportEntry).GetProperties().Select(p => p.Name).ToArray());
        foreach (object item in report)
        {
            table.AddRow(typeof(PuzzleReportEntry).GetProperties().Select(p => p.GetValue(item)?.ToString()??string.Empty).ToArray());
        }

        AnsiConsole.Write(table);
        return 0;
    }

}