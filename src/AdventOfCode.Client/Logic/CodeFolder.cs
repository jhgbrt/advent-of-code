using Microsoft.Extensions.Logging;

namespace AdventOfCode.Client.Logic;
class Folder
{
    private readonly DirectoryInfo dir;
    private readonly ILogger logger;
    public Folder(string location, ILogger logger) { dir = new DirectoryInfo(location); this.logger = logger; }
    public string GetFileName(string file) => Path.Combine(dir.FullName, file);
    public bool Exists => dir.Exists;
    public Task CreateIfNotExists()
    {
        if (!dir.Exists)
        {
            logger.LogTrace($"CREATE: {dir}");
            dir.Create();
        }
        return Task.CompletedTask;
    }
    public Task Delete()
    {
        logger.LogTrace($"DELETE: {dir}");
        dir.Delete(true);
        return Task.CompletedTask;
    }
    public Task<string> ReadFile(string name)
    {
        logger.LogTrace($"READ: {name}");
        return File.ReadAllTextAsync(name);
    }
    public Task WriteFile(string name, string content)
    {
        DeleteIfExists(name);
        logger.LogTrace($"WRITE: {name}");
        return File.WriteAllTextAsync(name, content);
    }
    public Task DeleteIfExists(string name)
    {
        var n = GetFileName(name);
        if (File.Exists(n))
        {
            logger.LogTrace($"DELETE: {n}");
            File.Delete(n);
        }
        return Task.CompletedTask;
    }
    public void CopyFile(FileInfo source)
    {
        var destination = GetFileName(source.Name);
        logger.LogTrace($"{source} -> {destination}");
        source.CopyTo(destination, true);
    }
    public void CopyFiles(IEnumerable<FileInfo> sources)
    {
        foreach (var source in sources) CopyFile(source);
    }
    public IEnumerable<FileInfo> GetFiles(string pattern)
    {
        return dir.GetFiles(pattern);
    }
}
class OutputFolder : Folder
{

    public OutputFolder(string location, ILogger logger) : base(location, logger) { }
    private string CODE => GetFileName("aoc.cs");
    private string INPUT => GetFileName("input.txt");
    private string CSPROJ => GetFileName("aoc.csproj");
    public FileInfo Code => new FileInfo(CODE);
    public FileInfo Input => new FileInfo(INPUT);
    public FileInfo CsProj => new FileInfo(CSPROJ);
    public async Task WriteCode(string code) => await WriteFile(CODE, code);
}


class CodeFolder : Folder
{
    private readonly int year;
    private readonly int day;
    private readonly ILogger logger;

    public CodeFolder(int year, int day, ILogger logger) : base(GetDirectoryName(year, day), logger)
    {
        this.year = year;
        this.day = day;
        this.logger = logger;
    }

    public string CODE => GetFileName("AoC.cs");
    public string INPUT => GetFileName("input.txt");
    public string SAMPLE => GetFileName("sample.txt");
    public FileInfo Input => new FileInfo(INPUT);
    public Task<string> ReadCode() => ReadFile(CODE);
    public Task WriteCode(string content) => WriteFile(CODE, content);
    public Task WriteInput(string content) => WriteFile(INPUT, content);
    public Task WriteSample(string content) => WriteFile(SAMPLE, content);

    public IEnumerable<FileInfo> GetCodeFiles() => GetFiles("*.cs").Where(f => !f.FullName.Equals(CODE, StringComparison.OrdinalIgnoreCase));
    static string GetDirectoryName(int year, int day) => Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}");
}

class TemplateFolder : Folder
{
    public TemplateFolder(ILogger logger) : base(Path.Combine(Environment.CurrentDirectory, "Template"), logger)
    {
    }
    private string CODE => GetFileName("AoC.cs.txt");
    private string CSPROJ => GetFileName("aoc.csproj");
    public FileInfo Code => new FileInfo(CODE);
    public FileInfo CsProj => new FileInfo(CSPROJ);
    public async Task<string> ReadCode(int year, int day) => (await ReadFile(CODE)).Replace("{YYYY}", year.ToString()).Replace("{DD}", day.ToString("00"));
}