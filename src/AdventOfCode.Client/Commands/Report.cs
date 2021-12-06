using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("show a list of all puzzles, their status (unlocked, answered), and the answers posted")]
class Report
{
    AoCClient client;
    public Report(AoCClient client)
    {
        this.client = client;
    }
    public record Options(
        [property: Description("Year (default: current year)")] int? year,
        [property: Description("Day (default: current day)")] int? day,
        [property: Description("Only list unsolved puzzles")] bool? unsolved);

    public async Task Run(Options options)
    {
        (var year, var day, var unsolved) = options;

        foreach ((var y, var d) in AoCLogic.Puzzles())
        {
            if (year.HasValue && year != y) continue;
            if (day.HasValue && day != d) continue;

            var puzzle = await client.GetPuzzleAsync(y, d);
            if ((unsolved??false) && puzzle.Status == Status.Completed) continue;

            Console.WriteLine((y, d, puzzle.Status, puzzle.Answer.part1, puzzle.Answer.part2));
        }
    }

}