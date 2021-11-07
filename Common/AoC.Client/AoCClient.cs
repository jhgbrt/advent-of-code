using HtmlAgilityPack;

using System.Net;

class AoCClient : IDisposable
{
    HttpClientHandler handler;
    HttpClient client;
    DirectoryInfo baseDirectory;

    public AoCClient(Uri baseAddress, string sessionCookie, DirectoryInfo baseDirectory)
    {
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(baseAddress, new Cookie("session", sessionCookie));

        handler = new HttpClientHandler
        {
            CookieContainer = cookieContainer
        };

        client = new HttpClient(handler)
        {
            BaseAddress = baseAddress
        };
        this.baseDirectory = baseDirectory;
    }

    public async Task<Puzzle> GetAsync(int year, int day, bool usecache = true)
    {
        var dir = Path.Combine(baseDirectory.FullName, $"{year}", $"{day:00}");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var htmlfile = Path.Combine(baseDirectory.FullName, dir, $"{year}-{day:00}.html");

        string html;
        if (!File.Exists(htmlfile) || !usecache)
        {
            var response = await client.GetAsync($"{year}/day/{day}");
            if (response.StatusCode == HttpStatusCode.NotFound) return Puzzle.Locked(year, day);
            html = await response.Content.ReadAsStringAsync();
            await File.WriteAllTextAsync(htmlfile, html);
        }
        else
        {
            html = await File.ReadAllTextAsync(htmlfile);
        }

        var inputfile = Path.Combine(baseDirectory.FullName, dir, $"input.{year}-{day:00}.txt");

        string input;
        if (!File.Exists(inputfile) || !usecache)
        {
            var response = await client.GetAsync($"{year}/day/{day}/input");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                input = await response.Content.ReadAsStringAsync();
                await File.WriteAllTextAsync(inputfile, input);
            }
            else
            {
                input = string.Empty;
            }
        }
        else
        {
            input = await File.ReadAllTextAsync(inputfile);
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


        var innerHtml = string.Join("", articles.Zip(answers.Select(a => a.ParentNode)).Select(n => n.First.InnerHtml + n.Second.InnerHtml));
        var innerText = string.Join("", articles.Zip(answers.Select(a => a.ParentNode)).Select(n => n.First.InnerText + n.Second.InnerText));

        var puzzle = Puzzle.Unlocked(year, day, innerHtml, innerText, input, answer);

        return puzzle;
    }

    public void Dispose()
    {
        client.Dispose();
        handler.Dispose();
    }
}
