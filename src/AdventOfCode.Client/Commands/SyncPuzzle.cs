using System.ComponentModel;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;
[Description("sync the data (specifically the posted answers) for a puzzle. Requires AOC_SESSION set as an environment variable")]
class SyncPuzzle
{
    private readonly AoCClient client;

    public SyncPuzzle(AoCClient client)
    {
        this.client = client;
    }
    public record Options(
        [property: Description("Year (default: current year)")] int? year,
        [property: Description("Day (default: current day)")] int? day
        );
    public async Task Run(Options options)
    {
        (var year, var day) = (options.year??DateTime.Now.Year, options.day??DateTime.Now.Day);

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Puzzle not yet unlocked");
            return;
        }

        var dir = AoCLogic.GetDirectory(year, day);
        if (!dir.Exists) 
        {
            Console.WriteLine("Puzzle not yet initialized. Use 'init' first.");
            return;
        }

        Console.WriteLine("Updating puzzle answers");
        var answers = AoCLogic.GetFileName(year, day, "answers.json");
        var puzzle = await client.GetPuzzleAsync(year, day, false);
        var answer = puzzle.Answer;
        File.WriteAllText(answers, JsonSerializer.Serialize(answer));
        Console.WriteLine(puzzle.Text);
    }
}
