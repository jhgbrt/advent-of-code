using Spectre.Console.Cli;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;

[Description("Verify the results for the given puzzle(s).")]
class Verify : Spectre.Console.Cli.AsyncCommand<Verify.Settings>
{
    AoCClient client;
    private readonly AoCManager manager;

    public Verify(AoCClient client, AoCManager manager)
    {
        this.client = client;
        this.manager = manager;
    }

    public enum Speed
    {
        all,
        normal,
        fast
    }

    public class Settings : AoCSettings
    {
        [Description("The fully qualified name of the type containing the code for this puzzle. " +
        "Use a format string with {0} and {1} as placeholders for year and day. " +
        "(default: AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00})")]
        [CommandOption("--typename")]
        public string? typeName { get; set; }
        [Description("Don't re-execute the puzzle if the result is cached")]
        [CommandOption("-c|--cache")]
        public bool? cache { get; set; }
        [Description("Don't re-execute puzzles that took > 1 second (if they're in the cache)")]
        [CommandOption("-s|--speed")]
        public Speed? speed { get; set; }
    }
    
    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {

        (var year, var day, var typeName, var cache, var speed) = (
              options.year
            , options.day
            , string.IsNullOrEmpty(options.typeName) ? "AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00}" : options.typeName
            , options.cache ?? false
            , options.speed ?? Speed.fast
            );

        var sw = Stopwatch.StartNew();
        foreach (var (y, d) in AoCLogic.Puzzles())
        {
            if (year.HasValue && year != y) continue;
            if (day.HasValue && day != d) continue;
            var puzzle = await client.GetPuzzleAsync(y, d);
            var cached = Cache.Exists(y,d,"result.json")
                ? JsonSerializer.Deserialize<DayResult>(await Cache.ReadFromCache(y, d, "result.json"))
                : null;
            if (cached != null && speed == Speed.fast && cached.Elapsed > TimeSpan.FromSeconds(1))
            {
                Write(puzzle, cached, true);
                continue;
            }
            var result = cache && cached is not null ? cached : await manager.Run(typeName, y, d);
            if (result is not null)
                Write(puzzle, result, false);
        }
        Console.WriteLine($"done. Total time: {sw.Elapsed}");
        return 0;
    }

    private static void Write(Puzzle puzzle, DayResult result, bool skipped)
    {
        (var duration, var dcolor) = result.Elapsed.TotalMilliseconds switch
        {
            < 10 => ("< 10 ms", Console.ForegroundColor),
            < 100 => ("< 100 ms", Console.ForegroundColor),
            < 1000 => ("< 1s", Console.ForegroundColor),
            double value when value < 3000 => ($"~ {(int)Math.Round(value / 1000)} s", ConsoleColor.Yellow),
            double value => ($"~ {(int)Math.Round(value / 1000)} s", ConsoleColor.Red)
        };


        var comparisonResult = puzzle.Compare(result);

        (var status, var color, var explanation) = comparisonResult switch
        {
            { part1: ResultStatus.Failed } or { part2: ResultStatus.Failed } => ("FAILED", ConsoleColor.Red, $"- expected {(puzzle.Answer.part1, puzzle.Answer.part2)} but was ({(result.part1.Value, result.part2.Value)})."),
            { part1: ResultStatus.AnsweredButNotImplemented } or { part2: ResultStatus.AnsweredButNotImplemented } => ("SKIPPED", ConsoleColor.Red, " - answered but no implementation."),
            { part1: ResultStatus.NotImplemented, part2: ResultStatus.NotImplemented } => ("SKIPPED", ConsoleColor.Yellow, " - not implemented."),
            { part1: ResultStatus.NotImplemented, part2: ResultStatus.Ok } => ("SKIPPED", ConsoleColor.Yellow, " - part 1 not implemented."),
            { part1: ResultStatus.Ok, part2: ResultStatus.NotImplemented } => ("SKIPPED", ConsoleColor.Yellow, " - part 2 not implemented."),
            _ => ("OK", ConsoleColor.Green, "")
        };

        if (skipped) 
            explanation += " Not verified, too slow; use --speed=all to include)";

        Console.Write($"{result.year}-{result.day:00}: ");
        Console.Write("[");
        Console.ForegroundColor = color;
        Console.Write(status);
        Console.ResetColor();
        Console.Write("]");
        Console.Write(" - ");
        Console.ForegroundColor = dcolor;
        Console.Write(duration);
        Console.ResetColor();
        Console.Write(explanation);
        Console.WriteLine();

    }
}



