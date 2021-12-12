using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.Diagnostics;

namespace AdventOfCode.Client.Commands;

[Description("Verify the results for the given puzzle(s).")]
class Verify : AsyncCommand<Verify.Settings>
{
    private readonly PuzzleManager manager;

    public Verify(PuzzleManager manager)
    {
        this.manager = manager;
    }

    public class Settings : AoCSettings
    {
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

        var sw = Stopwatch.StartNew();
        await AnsiConsole.Status()
            .StartAsync("Running...", async ctx =>
            {
                ctx.SpinnerStyle(Style.Parse("green"));

                foreach (var (y, d) in AoCLogic.Puzzles())
                {
                    if (!all && year != y) continue;
                    if (!all && day != d) continue;
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



