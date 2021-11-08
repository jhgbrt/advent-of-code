
using System.Text.Json;
using System.Text.Json.Serialization;

interface IPuzzleRepository
{
    public Task<Puzzle?> GetAsync(int year, int day);
    public Task PutAsync(Puzzle puzzle);
}

class PuzzleRepository : IPuzzleRepository
{
    readonly FileProvider provider;
    readonly JsonSerializerOptions options;
    public PuzzleRepository(DirectoryInfo dbdir)
    {
        this.provider = new FileProvider(dbdir);
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        this.options = options;
    }
    public async Task PutAsync(Puzzle puzzle) 
        => await provider.WriteAsync(puzzle.Year, puzzle.Day, "json", JsonSerializer.Serialize(puzzle, options));

    public async Task<Puzzle?> GetAsync(int year, int day)
    {
        if (provider.Exists(year, day, "json"))
        {
            var json = await provider.ReadAsync(year, day, "json");
            return JsonSerializer.Deserialize<Puzzle>(json, options);
        }
        return null;
    }
}