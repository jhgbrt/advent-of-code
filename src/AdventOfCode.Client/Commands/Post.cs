using Spectre.Console.Cli;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AdventOfCode.Client.Commands;

[Description("Post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable.")]
class Post : AsyncCommand<Post.Settings>
{
    private readonly AoCManager manager;

    public Post(AoCManager manager)
    {
        this.manager = manager;
    }
    public class Settings : AoCSettings
    {

        [Description("The solution to the current puzzle part"), Required]
        [CommandArgument(2, "<SOLUTION>")]
        public string? value { get; set; }
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day, var value) = (options.year??DateTime.Now.Year, options.day??DateTime.Now.Day, options.value);

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Invalid year/day combination. Use --year [YEAR] --day [DAY].");
            return 1;
        }

        if (string.IsNullOrEmpty(value))
        {
            Console.WriteLine("No value provided. Use --value [value]");
        }

        (var status, var reason, var part) = await manager.PreparePost(year, day);
        if (!status)
        {
            Console.WriteLine(reason);
            return 1;
        }

        var result = await manager.Post(year, day, part, value??string.Empty);

        Console.WriteLine(result.status);
        Console.WriteLine(result.content);
        return 0;

    }
}


