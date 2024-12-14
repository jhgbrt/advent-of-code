namespace Net.Code.AdventOfCode.Toolkit.Commands;

using Net.Code.AdventOfCode.Toolkit.Core;
using Net.Code.AdventOfCode.Toolkit.Infrastructure;
using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


[Description("Post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable.")]
class Post(IPuzzleManager manager, AoCLogic logic, IInputOutputService io) : SinglePuzzleCommand<Post.Settings>(logic)
{
    public class Settings : CommandSettings, IAoCSettings
    {

        [Description("Year (default: current year)")]
        [CommandArgument(0, "[YEAR]")]
        public int? year { get; set; }
        [Description("Day (default: current day)")]
        [CommandArgument(1, "[DAY]")]
        public int? day { get; set; }
        [Description("The solution to the current puzzle part (by default, solution is looked up in the stored results)")]
        [CommandArgument(2, "[SOLUTION]")]
        public string? value { get; set; }

    }
    public override async Task<int> ExecuteAsync(PuzzleKey key, Settings options)
    {
        var value = options.value;
        if (string.IsNullOrEmpty(value))
        {
            var result = await manager.GetPuzzleResult(key);
            if (result.result.Part1.Status == ResultStatus.Unknown)
            {
                value = result.result.Part1.Value;
            }
            else if (result.result.Part2.Status == ResultStatus.Unknown)
            {
                value = result.result.Part2.Value;
            }
            else if (result.result.Part1.Status == ResultStatus.NotImplemented)
            {
                io.MarkupLine($"[red]Part 1 has not been implemented for {key}[/]");
                return 1;
            }
            else if (result.result.Part2.Status == ResultStatus.NotImplemented)
            {
                io.MarkupLine($"[red]Part 2 has not been implemented for {key}[/]");
                return 1;
            }
            else if (result.Ok)
            {
                io.MarkupLine($"[red]Both parts have already been posted for {key}[/]");
                return 1;
            }
            else
            {
                io.MarkupLine("[red]No solution found for this puzzle[/]");
                return 1;
            }
        }

        var (success, content) = await manager.PostAnswer(key, value);

        var color = success ? Color.Green : Color.Red;
        io.MarkupLine($"[{color}]{content.EscapeMarkup()}[/]");
        return 0;
    }
}


