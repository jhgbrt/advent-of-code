namespace AdventOfCode.Client.Commands;

using Spectre.Console;
using Spectre.Console.Cli;

using System.ComponentModel;
using System.Text.Json;
using System.Xml.Linq;

[Description("Initialize the code for a specific puzzle. Requires AOC_SESSION set as an environment variable")]
class Init : AsyncCommand<Init.Settings>
{
    private readonly AoCClient client;

    public Init(AoCClient client)
    {
        this.client = client;
    }
    public class Settings : AoCSettings
    {
        [property: Description("Force (if true, refresh cache)")] 
        [CommandOption("-f|--force")]
        public bool? force { get; set; }

        public override ValidationResult Validate()
        {
            var result = base.Validate();
            if (result.Successful)
            {
                var dir = FileSystem.GetDirectory(year, day);
                if (dir.Exists && !(force??false))
                {
                    return ValidationResult.Error("Puzzle for {year}/{day} already initialized. Use --force to re-initialize.");
                }

            }
            return result;
        }
    }
    public override async Task<int> ExecuteAsync(CommandContext context, Settings options)
    {
        (var year, var day, var force) = (options.year, options.day, options.force??false);

        AnsiConsole.WriteLine("Puzzle is unlocked");

        var dir = FileSystem.GetDirectory(year, day);

        if (dir.Exists && force)
            dir.Delete(true);

        dir.Create();

        var aoc = FileSystem.GetFile(year, day, "AoC.cs");
        if (!aoc.Exists)
        {
            Console.WriteLine("Writing file: AoC.cs");
            File.WriteAllLines(
                Path.Combine(dir.FullName, "AoC.cs"),
                new[]
                {
                   $"namespace AdventOfCode.Year{year}.Day{day:00};",
                    "",
                   $"public class AoC{year}{day:00} : AoCBase",
                    "{",
                   $"    static string[] input = Read.InputLines(typeof(AoC{year}{day:00}));",
                    "    public override object Part1() => -1;",
                    "    public override object Part2() => -1;",
                    "}",
                });
        }

        var sample = FileSystem.GetFileName(year, day, "sample.txt");
        if (!File.Exists(sample))
        {
            File.Create(sample);
            AddEmbeddedResource(sample);
        }

        AnsiConsole.WriteLine("Retrieving puzzle input");

        var input = FileSystem.GetFileName(year, day, "input.txt");
        var content = await client.GetPuzzleInputAsync(year, day);
        await File.WriteAllTextAsync(input, content);
        AddEmbeddedResource(input);

        AnsiConsole.WriteLine("Retrieving puzzle data");

        var answers = FileSystem.GetFileName(year, day, "answers.json");
        var puzzle = await client.GetPuzzleAsync(year, day, !force);
        var answer = puzzle.Answer;
        await File.WriteAllTextAsync(answers, JsonSerializer.Serialize(answer));
        AddEmbeddedResource(answers);

        return 0;
    }

    void AddEmbeddedResource(string path)
    {
        var csproj = "aoc.csproj";
        var doc = XDocument.Load(csproj);
        var itemGroup = (
            from node in doc.Descendants()
            where node.Name == "ItemGroup"
            select node
            ).First();

        var relativePath = path.Substring(Environment.CurrentDirectory.Length + 1);
        if (!itemGroup.Elements().Select(e => e.Attribute("Include")).Where(a => a != null && a.Value == relativePath).Any())
        {
            var embeddedResource = new XElement("EmbeddedResource");
            embeddedResource.SetAttributeValue("Include", relativePath);
            itemGroup.Add(embeddedResource);
        }
        doc.Save(csproj);
    }

}


