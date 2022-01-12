
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

[Description("Export the code for a puzzle to a stand-alone C# project")]
class Export : AsyncCommand<Export.Settings>
{
    private readonly ICodeManager manager;

    public Export(ICodeManager manager)
    {
        this.manager = manager;
    }

    public class Settings : AoCSettings
    {
        [Description("output location. If empty, exported code is written to stdout")]
        [CommandOption("-o|--output")]
        public string? output { get; set; }
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day, var output) = (options.year, options.day, options.output);

        if (!year.HasValue || !day.HasValue)
            throw new Exception("Please specify year & day explicitly");

        string code = await manager.GenerateCodeAsync(year.Value, day.Value);

        if (string.IsNullOrEmpty(output))
        {
            AnsiConsole.WriteLine(code.EscapeMarkup());
        }
        else
        {
            AnsiConsole.WriteLine($"Exporting puzzle: {year}/{day} to {output}");
            await manager.ExportCode(year.Value, day.Value, code, output);
        }


        return 0;

    }


}



