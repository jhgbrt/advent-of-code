using AdventOfCode.Client;
using System.CommandLine;

var root = CommandHelper.CreateRootCommand();

await root.InvokeAsync(args);


