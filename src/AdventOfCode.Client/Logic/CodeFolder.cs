using Microsoft.Extensions.Logging;

namespace AdventOfCode.Client.Logic;

class CodeFolder
{
    private readonly DirectoryInfo dir;
    private readonly int year;
    private readonly int day;
    private readonly ILogger logger;

    public CodeFolder(int year, int day, ILogger logger)
    {
        this.dir = GetDirectory(year, day);
        this.year = year;
        this.day = day;
        this.logger = logger;
    }

    private string CODE => GetFileName(year, day, "AoC.cs");
    private string INPUT => GetFileName(year, day, "input.txt");
    private string SAMPLE => GetFileName(year, day, "sample.txt");

    private Task<string> ReadFile(string name)
    {
        logger.LogTrace($"READ: {name}");
        return File.ReadAllTextAsync(name);
    } 
    private Task WriteFile(string name, string content)
    {
        logger.LogTrace($"WRITE: {name}");
        return File.WriteAllTextAsync(name, content);
    }

    public Task<string> ReadCode() => ReadFile(CODE);
    public Task WriteCode(string content) => WriteFile(CODE, content);
    public Task<string> ReadInput() => ReadFile(INPUT);
    public Task WriteInput(string content) => WriteFile(INPUT, content);
    public Task<string> ReadSample() => ReadFile(SAMPLE);
    public Task WriteSample(string content) => WriteFile(SAMPLE, content);
    public bool Exists => dir.Exists;
    public void Create()
    {
        logger.LogTrace($"CREATE: {dir}");
        dir.Create();
    }

    public void Delete()
    {
        logger.LogTrace($"DELETE: {dir}");
        dir.Delete(true);
    }

    internal async Task ExportTo(string code, string output)
    {
        var publishLocation = new DirectoryInfo(output);
        if (!publishLocation.Exists)
        {
            logger.LogTrace($"CREATE: {publishLocation}");
            publishLocation.Create();
        }

        var aoccs = Path.Combine(publishLocation.FullName, "aoc.cs");
        if (File.Exists(aoccs))
        {
            logger.LogTrace($"DELETE: {aoccs}");
            File.Delete(aoccs);
        }

        logger.LogTrace($"WRITE: {aoccs}");
        await File.WriteAllTextAsync(aoccs, code);

        foreach (var file in dir.GetFiles("*.cs").Where(f => !f.FullName.Equals(CODE, StringComparison.OrdinalIgnoreCase)))
        {
            logger.LogTrace($"COPY: {file.FullName} -> {Path.Combine(publishLocation.FullName, file.Name)}");
            file.CopyTo(Path.Combine(publishLocation.FullName, file.Name), true);
        }

        var inputtxt = Path.Combine(publishLocation.FullName, "input.txt");
        logger.LogTrace($"COPY: {INPUT} -> {inputtxt}");
        File.Copy(INPUT, inputtxt, true);

        var tpl = new TemplateFolder();
        var csprojdest = Path.Combine(publishLocation.FullName, "aoc.csproj");
        logger.LogTrace($"COPY: {tpl.CsProj} -> {csprojdest}");
        tpl.CsProj.CopyTo(csprojdest, true);
    }

    static DirectoryInfo GetDirectory(int year, int day) => new(Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}"));
    static FileInfo GetFile(int year, int day, string fileName) => new(Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}", fileName));
    static string GetFileName(int year, int day, string fileName) => GetFile(year, day, fileName).FullName;
}