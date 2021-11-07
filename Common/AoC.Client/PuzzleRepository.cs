
using System.Text.Json;
using System.Text.Json.Serialization;

interface IPuzzleRepository
{
    public Task<Puzzle?> GetAsync(int year, int day);
    public Task PutAsync(Puzzle puzzle);
}

class PuzzleRepository : IPuzzleRepository
{
    DirectoryInfo dbdir;
    JsonSerializerOptions options;
    public PuzzleRepository(DirectoryInfo dbdir)
    {
        this.dbdir = dbdir;
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        this.options = options;
    }
    public async Task PutAsync(Puzzle puzzle)
    {
        var year = puzzle.Year;
        var day = puzzle.Day;
        var dir = Path.Combine(dbdir.FullName, $"{puzzle.Year}", $"{puzzle.Day:00}");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var puzzlefile = Path.Combine(dbdir.FullName, dir, $"{year}-{day}.json");
        await File.WriteAllTextAsync(puzzlefile, JsonSerializer.Serialize(puzzle, options));
    }

    public async Task<Puzzle?> GetAsync(int year, int day)
    {
        var dir = Path.Combine(dbdir.FullName, $"{year}", $"{day:00}");
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        var puzzlefile = Path.Combine(dbdir.FullName, dir, $"{year}-{day}.json");
        if (File.Exists(puzzlefile))
        {
            var json = await File.ReadAllTextAsync(puzzlefile);
            return JsonSerializer.Deserialize<Puzzle>(json, options);
        }

        return null;
    }
}