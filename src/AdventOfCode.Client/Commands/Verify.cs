using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.Diagnostics;

namespace AdventOfCode.Client.Commands;

[Description("Verify the results for the given puzzle(s).")]
class Verify : AsyncCommand<Verify.Settings>
{
    private readonly PuzzleManager manager;
    private readonly ReportManager reportManager;

    public Verify(PuzzleManager manager, ReportManager reportManager)
    {
        this.manager = manager;
        this.reportManager = reportManager;
    }

    public class Settings : CommandSettings
    {
        [Description("Year (default: current year)")]
        [CommandArgument(0, "[YEAR]")]
        public int? year { get; set; }
        [Description("Day (default: current day)")]
        [CommandArgument(1, "[DAY]")]
        public int? day { get; set; }
        [Description("The fully qualified name of the type containing the code for this puzzle. " +
        "Use a format string with {0} and {1} as placeholders for year and day. " +
        "(default: AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00})")]
        [CommandOption("--typename")]
        public string? typeName { get; set; }
        [Description("Force re-execution of slow puzzles")]
        [CommandOption("-f|--force")]
        public bool? force { get; set; }
        [Description("Verify all puzzles")]
        [CommandOption("-a|--all")]
        public bool all { get; set; }
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {

        (var year, var day, var typeName, var force, var all) = (
              options.year
            , options.day
            , string.IsNullOrEmpty(options.typeName) ? "AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00}" : options.typeName
            , options.force ?? false
            , options.all
            );

        if (options.all)
        {
            await AnsiConsole.Progress()
                .StartAsync(async ctx =>
                {

                    var tasks = AoCLogic.Years().Select(y => ctx.AddTask(y.ToString())).ToArray();

                    foreach (var (year, task) in AoCLogic.Years().Zip(tasks))
                    {
                        var days = AoCLogic.Days(year).ToArray();
                        foreach (var day in days)
                        {
                            var resultStatus = await manager.GetPuzzleResult(year, day, force, typeName, (_, _) => { });
                            task.Increment((double)100/ days.Length);
                        }
                    }
                });
            var report = await reportManager.GetPuzzleReport(null, null).ToListAsync();
            AnsiConsole.Write(report.ToTable());
            return 0;
        }


        var sw = Stopwatch.StartNew();
        await AnsiConsole.Status()
            .StartAsync("Running...", async ctx =>
            {
                ctx.SpinnerStyle(Style.Parse("green"));

                foreach (var (y, d) in AoCLogic.Puzzles())
                {
                    if (year.HasValue && year != y) continue;
                    if (day.HasValue && day != d) continue;
                    if (!all && !year.HasValue && !day.HasValue && !AoCLogic.IsToday(y, d)) continue;
                    ctx.Status($"Running puzzle: {y}-{d:00} (1)");
                    var resultStatus = await manager.GetPuzzleResult(y, d, force, typeName, (_, _) => ctx.Status($"Running puzzle: {y}-{d:00} (2)"));
                    var reportLine = resultStatus.ToReportLine();
                    AnsiConsole.MarkupLine(reportLine.ToString());

                }

            });
        AnsiConsole.WriteLine($"done. Total time: {sw.Elapsed}");
        return 0;
    }

}



