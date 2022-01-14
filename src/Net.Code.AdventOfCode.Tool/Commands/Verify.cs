
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

[Description("Verify the results for the given puzzle(s).")]
class Verify : ManyPuzzlesCommand<AoCSettings>
{
    private readonly IPuzzleManager manager;

    public Verify(IPuzzleManager manager)
    {
        this.manager = manager;
    }

    public override async Task ExecuteAsync(int year, int day, AoCSettings options)
    {
        var resultStatus = await manager.GetPuzzleResult(year, day, (part, result) => AnsiConsole.MarkupLine($"{year}-{day:00} part {part}: {result.Value} ({result.Elapsed})"));
        var reportLine = resultStatus.ToReportLine();
        AnsiConsole.MarkupLine(reportLine.ToString());
    }

}



