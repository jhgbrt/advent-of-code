namespace AdventOfCode.Client.Commands;

using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

[Description("Show the puzzle instructions.")]
class Show : AsyncCommand<Show.Settings>
{
    private readonly AoCClient client;

    public Show(AoCClient client)
    {
        this.client = client;
    }

    public class Settings : AoCSettings { }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day) = (options.year, options.day);

        var puzzle = await client.GetPuzzleAsync(year, day);

        AnsiConsole.WriteLine(puzzle.Text);
        return 0;
    }
}
