
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

[Description("Verify the results for the given puzzle(s).")]
class Verify : ManyPuzzlesCommand<AoCSettings>
{
    private readonly IPuzzleManager manager;
    private readonly IInputOutputService io;

    public Verify(IPuzzleManager manager, AoCLogic aocLogic, IInputOutputService io) : base(aocLogic)
    {
        this.manager = manager;
        this.io = io;
    }

    public override async Task ExecuteAsync(int year, int day, AoCSettings options)
    {
        var resultStatus = await manager.GetPuzzleResult(year, day, (part, result) => io.MarkupLine($"{year}-{day:00} part {part}: {result.Value} ({result.Elapsed})"));
        var reportLine = resultStatus.ToReportLine();
        io.MarkupLine(reportLine.ToString());
    }

}



