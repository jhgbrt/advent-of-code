using HtmlAgilityPack;

using System.Net;

class AoCClient : IDisposable
{
    readonly HttpClientHandler handler;
    readonly HttpClient client;
    readonly FileProvider provider;

    public AoCClient(Uri baseAddress, string sessionCookie, DirectoryInfo baseDirectory)
    {
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(baseAddress, new Cookie("session", sessionCookie));

        handler = new HttpClientHandler { CookieContainer = cookieContainer };

        client = new HttpClient(handler) { BaseAddress = baseAddress };

        this.provider = new FileProvider(baseDirectory);
    }

    public async Task<Puzzle> GetAsync(int year, int day, bool usecache = true)
    {
        string html;
        if (!provider.Exists(year, day, "html") || !usecache)
        {
            var response = await client.GetAsync($"{year}/day/{day}");
            if (response.StatusCode == HttpStatusCode.NotFound) return Puzzle.Locked(year, day);
            html = await response.Content.ReadAsStringAsync();
            await provider.WriteAsync(year, day, "html", html);
        }
        else
        {
            html = await provider.ReadAsync(year, day, "html");
        }

        string input;
        if (!provider.Exists(year, day, "txt") || !usecache)
        {
            var response = await client.GetAsync($"{year}/day/{day}/input");
            if (response.StatusCode == HttpStatusCode.OK)
            {
                input = await response.Content.ReadAsStringAsync();
                await provider.WriteAsync(year, day, "txt", input);
            }
            else
            {
                input = string.Empty;
            }
        }
        else
        {
            input = await provider.ReadAsync(year, day, "txt");
        }


        var document = new HtmlDocument();
        document.LoadHtml(html);

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

        return Puzzle.Unlocked(year, day, innerHtml, innerText, input, answer);
    }

    public void Dispose()
    {
        client.Dispose();
        handler.Dispose();
    }
}
