
using AdventOfCode.Client.Commands;

namespace AdventOfCode.Client;

static class Cache
{
    private static string BaseDir => Path.Combine(Environment.CurrentDirectory, ".cache");
    private static string GetDirectory(int? year, int? day)
    {
        var dir = (year, day) switch
        {
            (null, _) => BaseDir,
            (not null, null) => Path.Combine(BaseDir, year.Value.ToString()),
            (not null, not null) => Path.Combine(BaseDir, year.Value.ToString(), day.Value.ToString("00"))
        };
        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);
        return dir;
    }

    private static string GetFileName(int? year, int? day, string name) => Path.Combine(GetDirectory(year, day), name);
    internal static Task<string> ReadFromCache(int? year, int? day, string name) => File.ReadAllTextAsync(GetFileName(year, day, name));
    internal static Task WriteToCache(int? year, int? day, string name, string content) => File.WriteAllTextAsync(GetFileName(year, day, name), content);
    internal static bool Exists(int? year, int? day, string name) => File.Exists(GetFileName(year, day, name));
}

