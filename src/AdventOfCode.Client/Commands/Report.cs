using Spectre.Console.Cli;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Show a list of all puzzles, their status (unlocked, answered), and the answers posted.")]
class Report : AsyncCommand<Report.Settings>
{
    AoCClient client;
    public Report(AoCClient client)
    {
        this.client = client;
    }
    public class Settings : CommandSettings
    {
        [property: Description("Only list unsolved puzzles")]
        [CommandOption("--unsolved-only")]
        public bool? unsolved { get; set; }
    }

    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        var unsolved = options.unsolved;

        foreach ((var y, var d) in AoCLogic.Puzzles())
        {
            var puzzle = await client.GetPuzzleAsync(y, d);
            if ((unsolved??false) && puzzle.Status == Status.Completed) continue;
            Console.WriteLine((y, d, puzzle.Status, puzzle.Answer.part1, puzzle.Answer.part2));
        }
        return 0;
    }

}