namespace AdventOfCode.Client;

using AdventOfCode.Common;

using System.Xml.Linq;


class SyncPuzzle
{
    private readonly AoCClient client;

    public SyncPuzzle(AoCClient client)
    {
        this.client = client;
    }
    public record Options(int year, int day);
    public async Task Run(Options options)
    {
        (var year, var day) = options;

        var dir = AoCLogic.GetDirectory(year, day);
        if (!dir.Exists) dir.Create();

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

        var input = AoCLogic.GetFileName(year, day, "input.txt");
        if (!File.Exists(input))
        {
            File.Create(input);
            AddEmbeddedResource(input);
        }
        var sample = AoCLogic.GetFileName(year, day, "sample.txt");
        if (!File.Exists(sample))
        {
            File.Create(sample);
            AddEmbeddedResource(sample);
        }
        var answers = AoCLogic.GetFileName(year, day, "answers.json");
        if (!File.Exists(answers))
        {
            File.WriteAllText(answers, "{\"part1\":null,\"part2\":null}");
            AddEmbeddedResource(answers);
        }
        if (!AoCLogic.IsValidAndUnlocked(year, day))
        {
            Console.WriteLine("Puzzle not yet unlocked");
            return;
        }
        
        var content = File.ReadAllText(input);
        if (string.IsNullOrEmpty(content))
        {
            Console.WriteLine("Retrieving puzzle data");
            content = await client.GetPuzzleInputAsync(year, day);
            File.WriteAllText(input, content);
        }

        var puzzle = await client.GetPuzzleAsync(year, day);
        var answer = puzzle.Answer;
        File.WriteAllText(answers, JsonSerializer.Serialize(answer));
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

        if (!itemGroup.Elements().Select(e => e.Attribute("Include")).Where(a => a != null && a.Value == path).Any())
        {
            var embeddedResource = new XElement("EmbeddedResource");
            embeddedResource.SetAttributeValue("Include", path);
            itemGroup.Add(embeddedResource);
        }
        doc.Save(csproj);
    }

}



