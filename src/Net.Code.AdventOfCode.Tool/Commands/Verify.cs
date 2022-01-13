
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

    public class Settings : AoCSettings
    {
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        var sw = Stopwatch.StartNew();
        await PuzzleCommandHelper.RunMultiPuzzle(options, async (year, day) =>
        {
            var resultStatus = await manager.GetPuzzleResult(year, day, (part, result) => AnsiConsole.MarkupLine($"{year}-{day:00} part {part}: {result.Value} ({result.Elapsed})"));
            var reportLine = resultStatus.ToReportLine();
            AnsiConsole.MarkupLine(reportLine.ToString());
        });
        AnsiConsole.WriteLine($"done. Total time: {sw.Elapsed}");
        return 0;
    }

}



