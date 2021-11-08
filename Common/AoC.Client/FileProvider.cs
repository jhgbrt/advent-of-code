class FileProvider
{
    readonly DirectoryInfo baseDirectory;
    public FileProvider(DirectoryInfo baseDirectory)
    {
        this.baseDirectory = baseDirectory;
    }
    public bool Exists(int year, int day, string extension) => File.Exists(GetPath(year, day, extension));
    public async Task<string> ReadAsync(int year, int day, string extension) => await File.ReadAllTextAsync(GetPath(year, day, extension));
    public async Task WriteAsync(int year, int day, string extension, string content) => await File.WriteAllTextAsync(GetPath(year, day, extension), content);

    private string GetPath(int year, int day, string extension)
    {
        var dir = Path.Combine(baseDirectory.FullName, $"{year}", $"{day:00}");
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        return Path.Combine(dir, $"{year}-{day:00}.{extension}");
    }


}
