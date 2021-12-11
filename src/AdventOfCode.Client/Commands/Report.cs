using Spectre.Console.Cli;

using System.ComponentModel;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;

[Description("Show a list of all puzzles, their status (unlocked, answered), and the answers posted.")]
class Report : AsyncCommand<Report.Settings>
{
    AoCClient client;
    public Report(AoCClient client)
    {
        this.client = client;
    }
    public class Settings : AoCSettings
    {
        [property: Description("Only list unsolved puzzles")]
        public bool? unsolved { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day, var unsolved) = (options.year, options.day, options.unsolved);

        foreach ((var y, var d) in AoCLogic.Puzzles())
        {
            if (year.HasValue && year != y) continue;
            if (day.HasValue && day != d) continue;

            var puzzle = await client.GetPuzzleAsync(y, d);
            var result = JsonSerializer.Deserialize<DayResult>(await Cache.ReadFromCache(y, d, "result.json"));

            if ((unsolved??false) && puzzle.Status == Status.Completed) continue;

            Console.WriteLine((y, d, puzzle.Status, puzzle.Answer.part1, puzzle.Answer.part2));
        }
        return 0;
    }

}