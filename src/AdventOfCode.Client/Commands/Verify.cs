using BenchmarkDotNet.Running;

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;

[Description("Verify the results for the given puzzle(s).")]
class Verify : ICommand<Verify.Options>
{
    AoCClient client;

    public Verify(AoCClient client)
    {
        this.client = client;
    }

    public record Options(
        [property: Description("Year (default: current year)")] int? year,
        [property: Description("Day (default: current day)")] int? day,
        [property: Description("The fully qualified name of the type containing the code for this puzzle. " +
        "Use a format string with {0} and {1} as placeholders for year and day. " +
        "(default: AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00})")] string? typeName
        ) : IOptions;

    public async Task Run(Options options)
    {

        (var year, var day, var typeName) = (
              options.year
            , options.day
            , string.IsNullOrEmpty(options.typeName) ? "AdventOfCode.Year{0}.Day{1:00}.AoC{0}{1:00}" : options.typeName
            );

        Console.WriteLine($"{year}, day {day}");

        var assembly = Assembly.GetEntryAssembly();
        if (assembly == null) throw new Exception("no entry assembly?");
        
        Console.WriteLine(string.Format(typeName, year, day));
        var sw = Stopwatch.StartNew();
        List<(int year, int day, object result1, TimeSpan elapsed1, object result2, TimeSpan elapsed2)> result = new();
        foreach (var (y, d) in AoCLogic.Puzzles())
        {
            if (year.HasValue && year != y) continue;
            if (day.HasValue && day != d) continue;
            var type = assembly.GetType(string.Format(typeName, y, d));

            if (type is null)
            {
                Console.WriteLine($"Could not find type {typeName} for {year}, {day}. Use --typeName to override.");
                continue;
            }

            dynamic aoc = Activator.CreateInstance(type)!;

            var puzzle = await client.GetPuzzleAsync(y, d);

            var part1 = Run(() => aoc.Part1());
            var part2 = Run(() => aoc.Part2());
            var duration = part1.Elapsed + part2.Elapsed;
            Write(y, d, puzzle, part1, part2, duration);
            result.Add((y, d, part1.Value, part1.Elapsed, part2.Value, part2.Elapsed));
        }
        Console.WriteLine($"done. Total time: {sw.Elapsed}");

        File.WriteAllText(Path.Combine(".cache", "durations.json"), JsonSerializer.Serialize(result));
    }

    private static void Write(int y, int d, Puzzle puzzle, Result part1, Result part2, TimeSpan duration)
    {
        Console.Write($"{y}-{d:00}: ");
        if ((part1.Value.ToString(), part2.Value.ToString()) != (puzzle.Answer.part1?.ToString(), puzzle.Answer.part2?.ToString()))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[FAILED]");
            Console.ResetColor();
            Console.Write(" - ");
            if (duration > TimeSpan.FromSeconds(1))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            Console.Write($"({duration})");
            Console.ResetColor();
            Console.Write($" - expected {(puzzle.Answer.part1, puzzle.Answer.part2)} but was {(part1.Value, part2.Value)}");
            Console.WriteLine();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("[OK]");
            Console.ResetColor();
            Console.Write(" - ");
            if (duration > TimeSpan.FromSeconds(1))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            Console.Write($"({duration})");
            Console.ResetColor();
            Console.WriteLine();
        }
    }

    static Result Run(Func<object> f)
    {
        var sw = Stopwatch.StartNew();
        return new(f(), sw.Elapsed);
    }
}



