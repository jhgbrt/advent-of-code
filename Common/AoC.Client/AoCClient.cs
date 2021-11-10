using HtmlAgilityPack;

using System.Net;
using NodaTime;
using System.Text.Json;

class AoCClient : IDisposable
{
    readonly HttpClientHandler handler;
    readonly HttpClient client;
    readonly DirectoryInfo directory;

    public AoCClient(Uri baseAddress, string sessionCookie, DirectoryInfo baseDirectory)
    {
        var cookieContainer = new CookieContainer();
        cookieContainer.Add(baseAddress, new Cookie("session", sessionCookie));

        handler = new HttpClientHandler { CookieContainer = cookieContainer };

        client = new HttpClient(handler) { BaseAddress = baseAddress };

        this.directory = baseDirectory;
    }

    public async Task<LeaderBoard?> GetLeaderBoardAsync(int year, int id, bool usecache = true)
    {
        (var statusCode, var content) = await GetAsync($"{year}-{id}.json", $"{year}/leaderboard/private/view/{id}.json", usecache);
        if (statusCode != HttpStatusCode.OK || content.StartsWith("<")) 
            return null;
        return Deserialize(content);
    }

    private LeaderBoard Deserialize(string content)
    {
        var jobject = JsonDocument.Parse(content).RootElement;
        var enumerator = jobject.EnumerateObject();
        enumerator.MoveNext();
        var ownerid = int.Parse(enumerator.Current.Value.GetString()!);
        enumerator.MoveNext();
        var members = GetMembers(enumerator.Current.Value);
        var lb = new LeaderBoard(ownerid, 2020, members.ToArray());
        return lb;

        IEnumerable<Member> GetMembers(JsonElement element)
        {
            foreach (var item in element.EnumerateObject())
            {
                var member = item.Value;
                string name = string.Empty;
                int id = 0;
                int stars = 0, localScore = 0, globalScore = 0;
                Instant lastStarInstant = Instant.MinValue;
                IReadOnlyDictionary<int, DailyStars>? completions = null;
                foreach (var property in member.EnumerateObject())
                {
                    switch (property.Name)
                    {
                        case "name": name = property.Value.GetString()!; break;
                        case "id": id = int.Parse(property.Value.GetString()!); break;
                        case "stars" when property.Value.ValueKind == JsonValueKind.Number: stars = property.Value.GetInt32(); break;
                        case "local_score" when property.Value.ValueKind == JsonValueKind.Number: localScore = property.Value.GetInt32(); break;
                        case "global_score" when property.Value.ValueKind == JsonValueKind.Number: globalScore = property.Value.GetInt32(); break;
                        case "last_star_ts" when property.Value.ValueKind == JsonValueKind.Number: lastStarInstant = Instant.FromUnixTimeSeconds(property.Value.GetInt32()); break;
                        case "last_star_ts": break;
                        case "completion_day_level":
                            completions = GetCompletions(property).ToDictionary(x => x.Day);
                            break;
                        default: throw new Exception($"unhandled property: {property.Name}");
                    }
                }
                yield return new Member(id, name, stars, localScore, globalScore, lastStarInstant, completions??new Dictionary<int, DailyStars>());
            }
        }
        IEnumerable<DailyStars> GetCompletions(JsonProperty property)
        {
            foreach (var compl in property.Value.EnumerateObject())
            {
                var day = int.Parse(compl.Name);

                (Instant? i1, Instant? i2) = (null, null);
                foreach (var star in compl.Value.EnumerateObject())
                {
                    var instant = Instant.FromUnixTimeSeconds(star.Value.EnumerateObject().First().Value.GetInt32());
                    switch (int.Parse(star.Name))
                    {
                        case 1: i1 = instant; break;
                        case 2: i2 = instant; break;
                    }
                }
                yield return new DailyStars(day, i1, i2);
            }

        }
    }

    private async Task<(HttpStatusCode StatusCode, string Content)> GetAsync(string filename, string path, bool usecache)
    {
        string content;
        var filepath = Path.Combine(directory.FullName, filename);
        if (!File.Exists(filepath) || !usecache)
        {
            var response = await client.GetAsync(path);
            if (response.StatusCode != HttpStatusCode.OK)
                return (response.StatusCode, string.Empty);
            content = await response.Content.ReadAsStringAsync();
            await File.WriteAllTextAsync(filepath, content);
        }
        else
        {
            content = await File.ReadAllTextAsync(filepath);
        }
        return (HttpStatusCode.OK, content);
    }

    public async Task<Puzzle> GetPuzzleAsync(int year, int day, bool usecache = true)
    {
        HttpStatusCode statusCode;
        (statusCode, var html) = await GetAsync($"{year}-{day}.html", $"{year}/day/{day}", usecache);
        if (statusCode != HttpStatusCode.OK) return Puzzle.Locked(year, day);

        (statusCode, var input) = await GetAsync($"{year}-{day}-input.txt", $"{year}/day/{day}/input", usecache);

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
