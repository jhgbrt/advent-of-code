using HtmlAgilityPack;

using Microsoft.Extensions.Configuration;

using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

var config = new ConfigurationBuilder()
    .AddCommandLine(args)
    .AddUserSecrets<Program>()
    .Build();

var baseDirectory = config["baseDirectory"];
var cookieValue = config["AOC_SESSION"];
var yearStr = config["year"];
var dayStr = config["day"];

if (string.IsNullOrEmpty(baseDirectory) || !Directory.Exists(baseDirectory))
{
    Console.Error.WriteLine("provide valid base directory");
    return;
}

baseDirectory = new DirectoryInfo(baseDirectory).FullName;

if (string.IsNullOrEmpty(cookieValue))
{
    Console.Error.WriteLine("session cookie not found");
    return;
}

int year = -1;
int day = -1;
if (!string.IsNullOrEmpty(yearStr))
{
    year = int.Parse(yearStr);
}
if (!string.IsNullOrEmpty(dayStr))
{
    day = int.Parse(dayStr);
}

var baseAddress = new Uri("https://adventofcode.com");

var cookieContainer = new CookieContainer();
cookieContainer.Add(baseAddress, new Cookie("session", cookieValue));

using var handler = new HttpClientHandler 
{
    CookieContainer = cookieContainer
};

using var client = new HttpClient(handler)
{
    BaseAddress = baseAddress
};

var options = new JsonSerializerOptions();
options.Converters.Add(new JsonStringEnumConverter());

if (year < 0) for (year = 2015; year <= DateTime.Now.Year; year++)
    {
        for (day = 1; day <= 25; day++)
        {
            var result = await Handle(year, day);
            if (result.Status == Status.Locked) break;
        }
    }
else if (day < 0)
{
    for (day = 1; day <= 25; day++)
    {
        var result = await Handle(year, day);
        if (result.Status == Status.Locked) break;
    }
}
else
{
    var result = await Handle(year, day);
    Console.WriteLine(JsonSerializer.Serialize(result, options));
}


async Task<Puzzle> Handle(int year, int day)
{
    var dir = Path.Combine(baseDirectory, $"{year}", $"{day:00}");
    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

    var htmlfile = Path.Combine(baseDirectory, dir, $"{year}-{day:00}.html");

    string html;
    if (!File.Exists(htmlfile))
    {
        html = await GetContent($"{year}/day/{day}");
        if (!string.IsNullOrEmpty(html))
        {
            await WriteContent(htmlfile, html);
        }
        else
        {
            return Puzzle.Locked(year, day);
        }
    }

    var document = new HtmlDocument();
    document.Load(htmlfile);

    var articles = document.DocumentNode.SelectNodes("//article").ToArray();
    var answers = (
        from node in document.DocumentNode.SelectNodes("//p")
        where node.InnerText.StartsWith("Your puzzle answer was")
        select node.SelectSingleNode("code")
        ).ToArray();

    var answer = answers.Length switch
    {
        2 => new Answer(answers[0].InnerText, answers[1].InnerText),
        1 => new Answer(answers[0].InnerText, null),
        0 => new Answer(null, null),
        _ => throw new Exception($"expected 0, 1 or 2 answers, not {answers.Length}")
    };

    var answerfile = Path.Combine(baseDirectory, dir, $"{year}-{day:00}-answers.json");

    if (File.Exists(answerfile))
    {
        File.Delete(answerfile);
    }

    var inputfile = Path.Combine(baseDirectory, dir, $"input.{year}-{day:00}.txt");
    string input;
    if (!File.Exists(inputfile))
    {
        input = await GetContent($"{year}/day/{day}/input");
        await WriteContent(inputfile, input);
    }
    else
    {
        input = await File.ReadAllTextAsync(inputfile);
    }

    var innerHtml = string.Join("", articles.Zip(answers.Select(a => a.ParentNode)).Select(n => n.First.InnerHtml + n.Second.InnerHtml));
    var innerText = string.Join("", articles.Zip(answers.Select(a => a.ParentNode)).Select(n => n.First.InnerText + n.Second.InnerText));

    var puzzle = Puzzle.Unlocked(year, day, htmlfile, inputfile, innerHtml, innerText, input, answer);
    var puzzlefile = Path.Combine(baseDirectory, dir, $"{year}-{day}.json");

    if (File.Exists(puzzlefile))
        File.Delete(puzzlefile);

    if (!File.Exists(puzzlefile))
        await File.WriteAllTextAsync(puzzlefile, JsonSerializer.Serialize(puzzle, options));

    return puzzle;
}

async Task<string> GetContent(string path)
{
    var response = await client.GetAsync(path);
    if (response.StatusCode == HttpStatusCode.NotFound)
    {
        return string.Empty;
    }

    var content = await response.Content.ReadAsStringAsync();
    return content;
}

async Task WriteContent(string filename, string content)
{
    if (File.Exists(filename)) File.Delete(filename);
    await File.WriteAllTextAsync(filename, content);
}

record Answer(object? part1, object? part2) { public static Answer Empty => new Answer(null, null); }
record Puzzle(int Year, int Day, string Htmlfile, string Inputfile, string Html, string Text, string Input, Answer Answer, Status Status)
{
    public static Puzzle Locked(int year, int day) => new(year, day, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, Answer.Empty, Status.Locked);
    public static Puzzle Unlocked(int year, int day, string htmlfile, string inputfile, string html, string text, string input, Answer answer) => new(year, day, htmlfile, inputfile, html, text, input, answer, answer switch
    {
        { part1: null, part2: null} => Status.Unlocked,
        { part1: not null, part2: null } => Status.AnsweredPart1,
        { part1: not null, part2: not null} => Status.AnsweredPart2,
        _ => throw new Exception($"inconsistent state for {year}/{day}/{answer}")
    });
}

enum Status
{
    Locked,
    Unlocked,
    AnsweredPart1,
    AnsweredPart2
}