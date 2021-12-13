namespace AdventOfCode.Client.Commands;

using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

[Description("Show a list of all puzzles, their status (unlocked, answered), and the answers posted.")]
class Report : AsyncCommand<Report.Settings>
{
    ReportManager manager;
    public Report(ReportManager manager)
    {
        this.manager = manager;
    }
    public class Settings : CommandSettings
    {
        [Description($"Filter by status. Valid values: {nameof(ResultStatus.NotImplemented)}, {nameof(ResultStatus.AnsweredButNotImplemented)}, {nameof(ResultStatus.Failed)}")]
        [CommandOption("--status")]
        public ResultStatus? status { get; set; }
        [Description("Only include entries for puzzles that take longer than the indicated number of seconds")]
        [CommandOption("--slower-than")]
        public int? slowerthan { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        var report = await manager.GetPuzzleReport(options.status, options.slowerthan).ToListAsync();
        AnsiConsole.Write(report.ToTable());
        return 0;
    }

}

static class TableFactory
{
    public static Table ToTable(this IEnumerable<PuzzleReportEntry> report)
    {
        var table = new Table();

        table.AddColumns(
            new TableColumn(nameof(PuzzleReportEntry.year)).RightAligned(),
            new TableColumn(nameof(PuzzleReportEntry.day)).RightAligned(),
            new TableColumn(nameof(PuzzleReportEntry.answer1)).Width(10),
            new TableColumn(nameof(PuzzleReportEntry.result1)).Width(10),
            new TableColumn(nameof(PuzzleReportEntry.elapsed1)).RightAligned(),
            new TableColumn(nameof(PuzzleReportEntry.status1)).RightAligned(),
            new TableColumn(nameof(PuzzleReportEntry.answer2)).Width(10),
            new TableColumn(nameof(PuzzleReportEntry.result2)).Width(10),
            new TableColumn(nameof(PuzzleReportEntry.elapsed2)).RightAligned(),
            new TableColumn(nameof(PuzzleReportEntry.status2)).RightAligned(),
            new TableColumn(nameof(PuzzleReportEntry.elapsedTotal)).RightAligned()
            );

        foreach (var item in report)
        {
            table.AddRow(
                item.year.ToString(),
                item.day.ToString(),
                item.answer1,
                item.result1,
                item.elapsed1.ToHumanReadableString(),
                item.status1.ToString(),
                item.answer2,
                item.result2,
                item.elapsed2.ToHumanReadableString(),
                item.status2.ToString(),
                item.elapsedTotal.ToHumanReadableString()
                );
        }
        return table;
    }

}