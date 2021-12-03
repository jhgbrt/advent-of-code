namespace AdventOfCode.Client;

using System.Text.Json;
using System.Text.Json.Serialization;

interface IPuzzleRepository
{
    public Task<Puzzle?> GetAsync(int year, int day);
    public Task PutAsync(Puzzle puzzle);
}

class PuzzleRepository : IPuzzleRepository
{
    readonly DirectoryInfo directory;
    readonly JsonSerializerOptions options;
    public PuzzleRepository()
    {
        this.directory = new DirectoryInfo(Environment.CurrentDirectory);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        this.options = options;
    }
    public async Task PutAsync(Puzzle puzzle) 
        => await File.WriteAllTextAsync(GetPath(puzzle.Year, puzzle.Day), JsonSerializer.Serialize(puzzle, options));

    public async Task<Puzzle?> GetAsync(int year, int day)
    {
        if (File.Exists(GetPath(year, day)))
        {
            var json = await File.ReadAllTextAsync(GetPath(year, day));
            return JsonSerializer.Deserialize<Puzzle>(json, options);
        }
        return null;
    }

    private string GetPath(int year, int day)
    {
        var dir = Path.Combine(directory.FullName, $"Year{year}", $"Day{day:00}");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return Path.Combine(dir, $"{year}-{day:00}.json");
    }
}