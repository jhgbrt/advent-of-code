
using Microsoft.Extensions.Logging;

using Net.Code.AdventOfCode.Tool.Core;

namespace Net.Code.AdventOfCode.Tool.Logic;


class Cache : ICache
{
    ILogger<Cache> logger;

    public Cache(ILogger<Cache> logger)
    {
        this.logger = logger;
        var dir = new DirectoryInfo(BaseDir);
        if (!dir.Exists) dir.Create();
        dir.Attributes |= FileAttributes.Hidden;
    }

    private static string BaseDir => Path.Combine(Environment.CurrentDirectory, ".cache");
    private static string GetDirectory(int? year, int? day)
    {
        var path = (year, day) switch
        {
            (null, _) => BaseDir,
            (not null, null) => Path.Combine(BaseDir, year.Value.ToString()),
            (not null, not null) => Path.Combine(BaseDir, year.Value.ToString(), day.Value.ToString("00"))
        };

        var dir = new DirectoryInfo(path);
        if (!dir.Exists) dir.Create();
        return path;
    }

    private static string GetFileName(int? year, int? day, string name) => Path.Combine(GetDirectory(year, day), name);
    public Task<string> ReadFromCache(int? year, int? day, string name)
    {
        logger.LogTrace($"CACHE-READ: {year} - {day} - {name}");
        return File.ReadAllTextAsync(GetFileName(year, day, name));
    }

    public Task WriteToCache(int? year, int? day, string name, string content)
    {
        logger.LogTrace($"CACHE-WRITE: {year} - {day} - {name}");
        return File.WriteAllTextAsync(GetFileName(year, day, name), content);
    }

    public bool Exists(int? year, int? day, string name) => File.Exists(GetFileName(year, day, name));
}

