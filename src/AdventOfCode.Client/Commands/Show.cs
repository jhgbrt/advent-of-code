using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

[Description("Show the puzzle instructions.")]
class Show : ICommand<Show.Options>
{
    private readonly AoCClient client;

    public Show(AoCClient client)
    {
        this.client = client;
    }
    public record Options(
        [property: Description("Year (default: current year)")] int? year,
        [property: Description("Day (default: current day)")] int? day
        ) : IOptions;
    public async Task Run(Options options)
    {
        (var year, var day) = (options.year ?? DateTime.Now.Year, options.day ?? DateTime.Now.Day);

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Puzzle not yet unlocked");
            return;
        }

        var puzzle = await client.GetPuzzleAsync(year, day);

        Console.WriteLine(puzzle.Text);
    }
}
