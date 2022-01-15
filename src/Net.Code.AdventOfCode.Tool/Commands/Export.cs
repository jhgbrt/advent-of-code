
using Net.Code.AdventOfCode.Tool.Core;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;

namespace Net.Code.AdventOfCode.Tool.Commands;

[Description("Export the code for a puzzle to a stand-alone C# project")]
partial class Export : SinglePuzzleCommand<Export.Settings>
{
    private readonly ICodeManager manager;
    private readonly IInputOutputService output;

    public Export(ICodeManager manager, IInputOutputService output)
    {
        this.manager = manager;
        this.output = output;
    }

    public class Settings : AoCSettings
    {
        [Description("output location. If empty, exported code is written to stdout")]
        [CommandOption("-o|--output")]
        public string? output { get; set; }
    }
    public override async Task ExecuteAsync(int year, int day, Settings options)
    {
        var output = options.output;
        string code = await manager.GenerateCodeAsync(year, day);

        if (string.IsNullOrEmpty(output))
        {
            this.output.WriteLine(code.EscapeMarkup());
        }
        else
        {
            this.output.WriteLine($"Exporting puzzle: {year}/{day} to {output}");
            await manager.ExportCode(year, day, code, output);
        }
    }
}



