﻿namespace AdventOfCode.Client.Commands;

using AdventOfCode.Client.Logic;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


[Description("Post an answer for a puzzle part. Requires AOC_SESSION set as an environment variable.")]
class Post : AsyncCommand<Post.Settings>
{
    private readonly PuzzleManager manager;

    public Post(PuzzleManager manager)
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
        (var year, var day, var value) = (options.year, options.day, options.value);


        (var status, var reason, var part) = await manager.PreparePost(year, day);
        if (!status)
        {
            AnsiConsole.WriteLine(reason);
            return 1;
        }

        var result = await manager.Post(year, day, part, value??string.Empty);

        var color = result.success ? Color.Green : Color.Red;
        AnsiConsole.MarkupLine($"[{color}]{result.content.EscapeMarkup()}[/]");
        return 0;

    }
}


