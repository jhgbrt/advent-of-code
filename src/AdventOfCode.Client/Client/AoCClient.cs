namespace AdventOfCode.Client;

using HtmlAgilityPack;
using System.Net;
using NodaTime;
using System.Text.Json;
using System.Diagnostics;
interface IAoCClient
{

}
class AoCClient : IDisposable, IAoCClient
{
    readonly HttpClientHandler handler;
    readonly HttpClient client;
    readonly DirectoryInfo cacheDirectory;

    public AoCClient(Uri baseAddress, string sessionCookie)
    {
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(baseAddress, new Cookie("session", sessionCookie));

        handler = new HttpClientHandler { CookieContainer = cookieContainer };

        client = new HttpClient(handler) { BaseAddress = baseAddress };

        this.cacheDirectory = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, ".cache"));
        if (!cacheDirectory.Exists) cacheDirectory.Create();
    }

    public async Task<(HttpStatusCode status, string content)> PostAnswerAsync(int year, int day, int part, string value)
    {
        var formValues = new Dictionary<string, string>()
        {
            ["level"] = part.ToString(),
            ["answer"] = value.ToString()
        };
        var content = new FormUrlEncodedContent(formValues);
        var result = await PostAsync($"{year}/day/{day}/answer", content);

        var document = new HtmlDocument();
        document.LoadHtml(result.Content);
        var articles = document.DocumentNode.SelectNodes("//article").ToArray();

        return (result.StatusCode, articles.First().InnerText);
    }

    public async Task<LeaderBoard?> GetLeaderBoardAsync(int year, int id, bool usecache = true)
    {
        (var statusCode, var content) = await GetAsync($"{year}-{id}.json", $"{year}/leaderboard/private/view/{id}.json", usecache);
        if (statusCode != HttpStatusCode.OK || content.StartsWith("<")) 
            return null;
        return Deserialize(year, content);
    }

    private static LeaderBoard Deserialize(int year, string content)
    {
        var jobject = JsonDocument.Parse(content).RootElement;

        int ownerid = -1;
        IEnumerable<Member>? members = Enumerable.Empty<Member>();
        foreach(var p in jobject.EnumerateObject())
        {
            switch (p.Name)
            {
                case "owner_id":
                    ownerid = int.Parse(p.Value.GetString()!);
                    break;
                case "members":
                    members = GetMembers(p.Value);
                    break;
            }
        }

        var lb = new LeaderBoard(ownerid, year, members.ToArray());
        return lb;

        IEnumerable<Member> GetMembers(JsonElement element)
        {
            foreach (var item in element.EnumerateObject())
            {
                var member = item.Value;
                var result = new Member(0, string.Empty, 0, 0, 0, null, new Dictionary<int,DailyStars>());
                foreach (var property in member.EnumerateObject())
                {
                    result = property.Name switch
                    {
                        "name" => result with { Name = property.Value.GetString()! },
                        "id" when property.Value.ValueKind is JsonValueKind.Number => result with { Id = property.Value.GetInt32() },
                        "stars" when property.Value.ValueKind is JsonValueKind.Number => result with { TotalStars = property.Value.GetInt32() },
                        "local_score" when property.Value.ValueKind is JsonValueKind.Number => result with { LocalScore = property.Value.GetInt32() },
                        "global_score" when property.Value.ValueKind is JsonValueKind.Number => result with { GlobalScore = property.Value.GetInt32() },
                        "last_star_ts" when property.Value.ValueKind is JsonValueKind.Number => result with { LastStarTimeStamp = Instant.FromUnixTimeSeconds(property.Value.GetInt32()) },
                        "completion_day_level" => result with { Stars = GetCompletions(property).ToDictionary(x => x.Day) },
                        _ => result
                    };
                }
                yield return result;
            }
        }
        IEnumerable<DailyStars> GetCompletions(JsonProperty property)
        {
            foreach (var compl in property.Value.EnumerateObject())
            {
                var day = int.Parse(compl.Name);
                var ds = new DailyStars(day, null, null);
                foreach (var star in compl.Value.EnumerateObject())
                {
                    var instant = Instant.FromUnixTimeSeconds(star.Value.EnumerateObject().First().Value.GetInt32());
                    ds = int.Parse(star.Name) switch
                    {
                        1 => ds with { FirstStar = instant },
                        2 => ds with { SecondStar= instant },
                        _ => ds,
                    };
                }
                yield return ds;
            }

        }
    }

    private async Task<(HttpStatusCode StatusCode, string Content)> GetAsync(string filename, string path, bool usecache)
    {
        string content;
        var filepath = Path.Combine(cacheDirectory.FullName, filename);
        if (!File.Exists(filepath) || !usecache)
        {
            var response = await client.GetAsync(path);
            content = await response.Content.ReadAsStringAsync();
            Trace.WriteLine($"GET: {path} - {response.StatusCode}");
            Trace.WriteLine($"{content}");
            if (response.StatusCode != HttpStatusCode.OK)
                return (response.StatusCode, content);
            await File.WriteAllTextAsync(filepath, content);
        }
        else
        {
            Trace.WriteLine($"CACHE: {path}");
            content = await File.ReadAllTextAsync(filepath);
        }
        return (HttpStatusCode.OK, content);
    }
    private async Task<(HttpStatusCode StatusCode, string Content)> PostAsync(string path, HttpContent body)
    {
        var response = await client.PostAsync(path, body);
        Trace.WriteLine($"GET: {path} - {response.StatusCode}");
        var content = await response.Content.ReadAsStringAsync();
        return (response.StatusCode, content);
    }

    public async Task<string> GetPuzzleInputAsync(int year, int day)
    {
        (var statusCode, var input) = await GetAsync($"{year}-{day}-input.txt", $"{year}/day/{day}/input", true);
        if (statusCode != HttpStatusCode.OK) return string.Empty;
        return input;
    }

    public async Task<Puzzle> GetPuzzleAsync(int year, int day, bool usecache = true)
    {
        HttpStatusCode statusCode;
        (statusCode, var html) = await GetAsync($"{year}-{day}.html", $"{year}/day/{day}", usecache);
        if (statusCode != HttpStatusCode.OK) return Puzzle.Locked(year, day);

        var input = await GetPuzzleInputAsync(year, day);

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
