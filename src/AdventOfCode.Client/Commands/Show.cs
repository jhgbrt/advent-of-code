﻿using Spectre.Console.Cli;

using System.ComponentModel;

namespace AdventOfCode.Client.Commands;

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

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Puzzle not yet unlocked");
            return 1;
        }

        var puzzle = await client.GetPuzzleAsync(year, day);

        Console.WriteLine(puzzle.Text);
        return 0;
    }
}
