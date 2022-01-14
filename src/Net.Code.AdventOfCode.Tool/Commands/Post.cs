namespace Net.Code.AdventOfCode.Tool.Commands;

using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


[Description("Post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable.")]
class Post : SinglePuzzleCommand<Post.Settings>
{
    private readonly IPuzzleManager manager;

    public Post(IPuzzleManager manager)
    {
        this.manager = manager;
    }
    public class Settings : CommandSettings, IAoCSettings
    {

        [Description("The solution to the current puzzle part"), Required]
        [CommandArgument(0, "<SOLUTION>")]
        public string? value { get; set; }
        [Description("Year (default: current year)")]
        [CommandArgument(1, "[YEAR]")]
        public int? year { get; set; } = DateTime.Now.Year;
        [Description("Day (default: current day)")]
        [CommandArgument(2, "[DAY]")]
        public int? day { get; set; } = DateTime.Now.Day;

    }
    public override async Task ExecuteAsync(int year, int day, Settings options)
    {
        (var status, var reason, var part) = await manager.PreparePost(year, day);
        if (!status)
        {
            AnsiConsole.WriteLine(reason);
            return;
        }
        var result = await manager.Post(year, day, part, options.value ?? string.Empty);

        var color = result.success ? Color.Green : Color.Red;
        AnsiConsole.MarkupLine($"[{color}]{result.content.EscapeMarkup()}[/]");
    }
}


