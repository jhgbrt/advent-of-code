
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

public class AoCSettings : CommandSettings
{
    [Description("Year (default: current year)")]
    [CommandArgument(0, "[YEAR]")]
    public int? year { get; set; }
    [Description("Day (default during advent: current day, null otherwise)")]
    [CommandArgument(1, "[DAY]")]
    public int? day { get; set; }

}
