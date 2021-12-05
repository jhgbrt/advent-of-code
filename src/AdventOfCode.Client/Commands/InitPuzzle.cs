namespace AdventOfCode.Client;

using System.ComponentModel;
using System.Text.Json;
using System.Xml.Linq;

[Description("initialize the code for a specific puzzle. Requires AOC_SESSION set as an environment variable")]
class InitPuzzle
{
    private readonly AoCClient client;

    public InitPuzzle(AoCClient client)
    {
        this.client = client;
    }
    public record Options(
        [property: Description("Year (default: current year)")] int? year,
        [property: Description("Day (default: current day)")] int? day,
        [property: Description("Force (if true, refresh cache)")]bool? force);
    public async Task Run(Options options)
    {
        (var year, var day, var force) = (options.year??DateTime.Now.Year, options.day??DateTime.Now.Day, options.force??false);

        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Puzzle not yet unlocked");
            return;
        }

        Console.WriteLine("Puzzle is unlocked");

        var dir = AoCLogic.GetDirectory(year, day);
        if (dir.Exists && !force)
        {
            Console.WriteLine("Puzzle for {year}/{day} already initialized. Use --force to re-initialize.");
            return;
        }

        if (dir.Exists && force)
            dir.Delete(true);

        dir.Create();

        var aoc = AoCLogic.GetFile(year, day, "AoC.cs");
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

        var sample = AoCLogic.GetFileName(year, day, "sample.txt");
        if (!File.Exists(sample))
        {
            File.Create(sample);
            AddEmbeddedResource(sample);
        }

        Console.WriteLine("Retrieving puzzle input");

        var input = AoCLogic.GetFileName(year, day, "input.txt");
        var content = await client.GetPuzzleInputAsync(year, day);
        File.WriteAllText(input, content);
        AddEmbeddedResource(input);

        Console.WriteLine("Retrieving puzzle data");

        var answers = AoCLogic.GetFileName(year, day, "answers.json");
        var puzzle = await client.GetPuzzleAsync(year, day, !force);
        var answer = puzzle.Answer;
        File.WriteAllText(answers, JsonSerializer.Serialize(answer));
        AddEmbeddedResource(answers);

        Console.WriteLine(puzzle.Text);
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


