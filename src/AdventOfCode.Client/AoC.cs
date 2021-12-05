namespace AdventOfCode.Client;

using System.CommandLine;

public static class AoC
{
    public static async Task InvokeAsync(string[] args)
    {
        var cmd = CommandHelper.CreateRootCommand();
        await cmd.InvokeAsync(args.Any() ? args : new[] { "run" });
    }
}




