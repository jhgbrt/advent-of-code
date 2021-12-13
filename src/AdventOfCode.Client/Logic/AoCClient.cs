namespace AdventOfCode.Client.Logic;

using HtmlAgilityPack;
using System.Net;
using NodaTime;
using System.Text.Json;
using System.Diagnostics;
using System.Text.RegularExpressions;

record Configuration(string BaseAddress, string SessionCookie);

class AoCClient : IDisposable
{
    readonly HttpClientHandler handler;
    readonly HttpClient client;

    public AoCClient(Configuration configuration)
    {
        var baseAddress = new Uri(configuration.BaseAddress);
        var sessionCookie = configuration.SessionCookie;

        var cookieContainer = new CookieContainer();
        cookieContainer.Add(baseAddress, new Cookie("session", sessionCookie));

        handler = new HttpClientHandler { CookieContainer = cookieContainer };

        client = new HttpClient(handler) { BaseAddress = baseAddress };
    }

    public async Task<(HttpStatusCode status, string content)> PostAnswerAsync(int year, int day, int part, string value)
    {
        var formValues = new Dictionary<string, string>()
        {
            ["level"] = part.ToString(),
            ["answer"] = value
        };
        var content = new FormUrlEncodedContent(formValues);
        var result = await PostAsync($"{year}/day/{day}/answer", content);

        var document = new HtmlDocument();
        document.LoadHtml(result.Content);
        var articles = document.DocumentNode.SelectNodes("//article").ToArray();

        return (result.StatusCode, articles.First().InnerText);
    }

    public async Task<LeaderBoard?> GetLeaderBoardAsync(int year, bool usecache = true)
    {
        var id = await GetMemberId();
        (var statusCode, var content) = await GetAsync(year, null, $"leaderboard-{id}.json", $"{year}/leaderboard/private/view/{id}.json", usecache);
        if (statusCode != HttpStatusCode.OK || content.StartsWith("<"))
            return null;
        return Deserialize(year, content);
    }

    public async Task<Member?> GetMemberAsync(int year, bool usecache = true)
    {
        var id = await GetMemberId();
        var lb = await GetLeaderBoardAsync(year, usecache);
        if (lb is null) return null;
        return lb.Members[id];
    }

    private static LeaderBoard Deserialize(int year, string content)
    {
        var jobject = JsonDocument.Parse(content).RootElement;

        int ownerid = -1;
        IEnumerable<Member>? members = Enumerable.Empty<Member>();
        foreach (var p in jobject.EnumerateObject())
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

        return new LeaderBoard(ownerid, year, members.ToDictionary(m => m.Id));

        IEnumerable<Member> GetMembers(JsonElement element)
        {
            foreach (var item in element.EnumerateObject())
            {
                var member = item.Value;
                var result = new Member(0, string.Empty, 0, 0, 0, null, new Dictionary<int, DailyStars>());
                foreach (var property in member.EnumerateObject())
                {
                    result = property.Name switch
                    {
                        "name" => result with { Name = property.Value.GetString()! },
                        "id" => result with { Id = int.Parse(property.Value.GetString() ?? throw new Exception("Invalid member id")) },
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
                        2 => ds with { SecondStar = instant },
                        _ => ds,
                    };
                }
                yield return ds;
            }

        }
    }

    private async Task<(HttpStatusCode StatusCode, string Content)> GetAsync(int? year, int? day, string name, string path, bool usecache)
    {
        string content;
        if (!Cache.Exists(year, day, name) || !usecache)
        {
            var response = await client.GetAsync(path);
            content = await response.Content.ReadAsStringAsync();
            Trace.WriteLine($"GET: {path} - {response.StatusCode}");
            Trace.WriteLine($"{content}");
            if (response.StatusCode != HttpStatusCode.OK) return (response.StatusCode, content);
            await Cache.WriteToCache(year, day, name, content);
        }
        else
        {
            Trace.WriteLine($"CACHE: {path}");
            content = await Cache.ReadFromCache(year, day, name);
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
        (var statusCode, var input) = await GetAsync(year, day, "input.txt", $"{year}/day/{day}/input", true);
        if (statusCode != HttpStatusCode.OK) return string.Empty;
        return input;
    }
    public async IAsyncEnumerable<Puzzle> GetPuzzlesAsync(IEnumerable<(int year, int day)> puzzles)
    {
        foreach (var (year, day) in puzzles)
            yield return await GetPuzzleAsync(year, day, true);
    }

    public async Task<Puzzle> GetPuzzleAsync(int year, int day, bool usecache = true)
    {
        HttpStatusCode statusCode;
        (statusCode, var html) = await GetAsync(year, day, "puzzle.html", $"{year}/day/{day}", usecache);
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
            1 => new Answer(answers[0].InnerText, string.Empty),
            0 => Answer.Empty,
            _ => throw new Exception($"expected 0, 1 or 2 answers, not {answers.Length}")
        };

        var innerHtml = string.Join("", articles.Zip(answers.Select(a => a.ParentNode)).Select(n => n.First.InnerHtml + n.Second.InnerHtml));
        var innerText = string.Join("", articles.Zip(answers.Select(a => a.ParentNode)).Select(n => n.First.InnerText + n.Second.InnerText));

        return Puzzle.Unlocked(year, day, innerHtml, innerText, input, answer);
    }

    public async Task<int> GetLeaderboardId()
    {
        (var statusCode, var html) = await GetAsync(null, null, "leaderboard.html", $"{DateTime.Now.Year}/leaderboard/private", true);
        if (statusCode != HttpStatusCode.OK) return 0;

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var text = (from node in document.DocumentNode.SelectNodes("//p")
                    where node.InnerText.StartsWith("You are a member")
                    select node.InnerText).Single();

        return int.Parse(Regex.Match(text, @"#(?<id>\d+)\)").Groups["id"].Value);
    }

    public async Task<int> GetMemberId()
    {
        (var statusCode, var html) = await GetAsync(null, null, "settings.html", "/settings", true);
        if (statusCode != HttpStatusCode.OK) return 0;

        var document = new HtmlDocument();
        document.LoadHtml(html);

        var text = (from node in document.DocumentNode.SelectNodes("//span")
                    where node.InnerText.Contains("anonymous user #")
                    select node.InnerText).Single();

        return int.Parse(Regex.Match(text, @"#(?<id>\d+)\)").Groups["id"].Value);
    }

    public void Dispose()
    {
        client.Dispose();
        handler.Dispose();
    }
}
