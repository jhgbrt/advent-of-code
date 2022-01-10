
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.Diagnostics;

namespace Net.Code.AdventOfCode.Tool.Commands;

[Description("Verify the results for the given puzzle(s).")]
class Verify : AsyncCommand<Verify.Settings>
{
    private readonly IPuzzleManager manager;

    public Verify(IPuzzleManager manager)
    {
        this.manager = manager;
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
        [Description("Verify all puzzles")]
        [CommandOption("-a|--all")]
        public bool all { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {

        (var year, var day, var typeName, var all) = (
              options.year
            , options.day
            , options.typeName
            , options.all
            );

        var sw = Stopwatch.StartNew();
        foreach (var (y, d) in AoCLogic.Puzzles())
        {
            if (year.HasValue && year != y) continue;
            if (day.HasValue && day != d) continue;
            if (!all && !year.HasValue && !day.HasValue && !AoCLogic.IsToday(y, d)) continue;

            var resultStatus = await manager.GetPuzzleResult(y, d, true, typeName,
                (part, result) => AnsiConsole.MarkupLine($"{y}-{d:00} part {part}: {result.Value} ({result.Elapsed})"));

            var reportLine = resultStatus.ToReportLine();
            AnsiConsole.MarkupLine(reportLine.ToString());

        }
        AnsiConsole.WriteLine($"done. Total time: {sw.Elapsed}");
        return 0;
    }

}



