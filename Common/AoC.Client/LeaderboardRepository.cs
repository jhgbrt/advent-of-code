using System.Text.Json;
using System.Text.Json.Serialization;

interface ILeaderboardRepository
{
    public Task<LeaderBoard?> GetAsync(int year, int id);
    public Task PutAsync(LeaderBoard lb);
}

class LeaderboardRepository : ILeaderboardRepository
{
    readonly DirectoryInfo directory;
    readonly JsonSerializerOptions options;
    public LeaderboardRepository(DirectoryInfo directory)
    {
        this.directory = directory;
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        this.options = options;
    }

    public async Task<LeaderBoard?> GetAsync(int year, int id)
    {
        if (File.Exists(GetPath(year, id)))
        {
            var json = await File.ReadAllTextAsync(GetPath(year, id));
            return JsonSerializer.Deserialize<LeaderBoard>(json, options);
        }
        return null;
    }
    public async Task PutAsync(LeaderBoard lb)
    {
        var path = GetPath(lb.Year, lb.OwnerId);
        Console.WriteLine(path);
        await File.WriteAllTextAsync(path, JsonSerializer.Serialize(lb, options));
    }

    private string GetPath(int year, int id)
    {
        var dir = Path.Combine(directory.FullName, $"{year}");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return Path.Combine(dir, $"{id:00}.json");
    }
}
