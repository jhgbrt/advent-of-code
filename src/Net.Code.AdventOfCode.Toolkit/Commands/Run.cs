
using Net.Code.AdventOfCode.Toolkit.Core;
using Net.Code.AdventOfCode.Toolkit.Infrastructure;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.Diagnostics;
using System.Text;

namespace Net.Code.AdventOfCode.Toolkit.Commands;

[Description("Run the code for a specific puzzle.")]
class Run(IAoCRunner manager, IPuzzleManager puzzleManager, AoCLogic aocLogic, IInputOutputService io) : ManyPuzzlesCommand<Run.Settings>(aocLogic)
{
    public class Settings : AoCSettings
    {
        [Description("The fully qualified name of the type containing the code for this puzzle. " +
        "Use a format string with {0} and {1} as placeholders for year and day. " +
        "(example: MyAdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00})")]
        [CommandOption("-t|--typename")]
        public string? typeName { get; set; }
    }

    public override async Task<int> ExecuteAsync(PuzzleKey key, Settings options, CancellationToken ct)
    {
        var typeName = options.typeName;

        var puzzle = await puzzleManager.GetPuzzle(key);

        var result = await manager.Run(typeName, key, (part, result) => io.MarkupLine($"part {part}: {result.Value} ({result.Elapsed.FormatTimeSpan()} - {result.bytes.FormatBytes()})"));

        if (result is not null)
        {
            await puzzleManager.AddResult(result);
            var resultStatus = await puzzleManager.GetPuzzleResult(key);
            var reportLine = resultStatus.ToReportLineMarkup();
            io.MarkupLine(reportLine);
        }
        else
        {
            io.MarkupLine("not implemented");
        }

        return 0;
    }
}


class PerfStats
{
    Stopwatch sw = Stopwatch.StartNew();
    long bytes = GC.GetTotalAllocatedBytes();
    public TimeSpan Elapsed => sw.Elapsed;
    public long Bytes => GC.GetTotalAllocatedBytes() - bytes;
    public PerfStats()
    {
        _ticks = [("start", sw.Elapsed, Bytes)];
    }

    List<(string label, TimeSpan elapsed, long bytes)> _ticks;
    public void Tick(string label)
    {
        _ticks.Add((label, Elapsed, Bytes));
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var ((l1, e1, b1), (label, e2, b2)) in _ticks.Zip(_ticks.Skip(1)))
        {
            sb.AppendLine($"{label}: {FormatTime(e2-e1)} - {FormatBytes(b2 - b1)}");
        }
        return sb.ToString();
    }


    static string FormatBytes(long b)
    {
        double bytes = b;
        string[] sizes = ["B", "KB", "MB", "GB", "TB"];
        int n = 0;
        while (bytes >= 1024 && n < sizes.Length - 1)
        {
            n++;
            bytes /= 1024;
        }
        return $"{bytes:0.00} {sizes[n]}";
    }
    static string FormatTime(TimeSpan timespan) => timespan switch
    {
        { TotalHours: > 1 } ts => $@"{ts:hh\:mm\:ss}",
        { TotalMinutes: > 1 } ts => $@"{ts:mm\:ss}",
        { TotalSeconds: > 10 } ts => $"{ts.TotalSeconds} s",
        { TotalSeconds: > 1 } ts => $@"{ts:ss\.fff} s",
        { TotalMilliseconds: > 1 } ts => $"{ts.TotalMilliseconds:0} ms",
        { TotalMicroseconds: > 1 } ts => $"{ts.TotalMicroseconds:0} μs",
        TimeSpan ts => $"{ts.TotalNanoseconds:0} ns"
    };
}