using System.ComponentModel;

namespace AdventOfCode.Client;

[Description("post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable")]
class PostSolution
{
    private readonly AoCClient client;

    public PostSolution(AoCClient client)
    {
        this.client = client;
    }
    public record Options(
        [property: Description("Year (default: current year)")] int? year,
        [property: Description("Day (default: current day)")] int? day,
        [property: Description("The solution to the puzzle part")] string value);
    public async Task Run(Options options)
    {
        (var year, var day, var value) = (options.year??DateTime.Now.Year, options.day??DateTime.Now.Day, options.value);

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Invalid year/day combination. Use --year [YEAR] --day [DAY].");
            return;
        }

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine("No value provided. Use --value [value]");
        }

        var puzzle = await client.GetPuzzleAsync(year, day);
        if (puzzle.Status == Status.Locked)
        {
            // should not happen
            Console.WriteLine("Puzzle is locked. Did you initialize it?");
            return;
        }
        if (puzzle.Status == Status.Completed)
        {
            Console.WriteLine("Already completed");
            return;
        }

        var part = puzzle.Status == Status.Unlocked ? 1 : 2;


        var result = await client.PostAnswerAsync(year, day, part, value);

        Console.WriteLine(result.status);
        Console.WriteLine(result.content);
    }
}


