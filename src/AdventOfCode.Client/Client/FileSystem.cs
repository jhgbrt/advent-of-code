
namespace AdventOfCode.Client;

static class FileSystem
{
    internal static DirectoryInfo GetDirectory(int year, int day) => new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}"));
    internal static FileInfo GetFile(int year, int day, string fileName) => new FileInfo(Path.Combine(Environment.CurrentDirectory, $"Year{year}", $"Day{day:00}", fileName));
    internal static string GetFileName(int year, int day, string fileName) => GetFile(year, day, fileName).FullName;
    internal static bool Exists(int year, int day, string filename) => File.Exists(GetFileName(year, day, filename));
}
