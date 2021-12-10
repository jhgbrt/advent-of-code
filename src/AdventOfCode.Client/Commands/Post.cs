using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;

[Description("Post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable.")]
class Post : ICommand<Post.Options>
{
    private readonly AoCManager manager;

    public Post(AoCManager manager)
    {
        this.manager = manager;
    }
    public record Options(
        [property: Description("Year (default: current year)")] int? year,
        [property: Description("Day (default: current day)")] int? day,
        [property: Description("The solution to the puzzle part"), Required] string value) : IOptions;
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

        (var status, var reason, var part) = await manager.PreparePost(year, day);
        if (!status)
        {
            Console.WriteLine(reason);
            return;
        }

        var result = await manager.Post(year, day, part, value);

        Console.WriteLine(result.status);
        Console.WriteLine(result.content);

    }
}


