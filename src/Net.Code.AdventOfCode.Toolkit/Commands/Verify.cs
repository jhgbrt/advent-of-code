namespace Net.Code.AdventOfCode.Toolkit.Commands;

using Net.Code.AdventOfCode.Toolkit.Core;
using Net.Code.AdventOfCode.Toolkit.Infrastructure;
using System.ComponentModel;
using System.Threading;

[Description("Verify the results for the given puzzle(s). Does not run the puzzle code.")]
class Verify(IPuzzleManager manager, AoCLogic aocLogic, IInputOutputService io) : ManyPuzzlesCommand<AoCSettings>(aocLogic)
{
    public override async Task<int> ExecuteAsync(PuzzleKey key, AoCSettings options, CancellationToken ct)
    {
        var resultStatus = await manager.GetPuzzleResult(key);
        if (resultStatus != null)
        {
            var reportLine = resultStatus.ToReportLineMarkup();
            io.MarkupLine(reportLine);
            return resultStatus.Ok ? 0 : 1;
        }
        else
        {
            io.MarkupLine($"[yellow]No result found for {key}.[/]");
            return 1;
        }
    }

}

