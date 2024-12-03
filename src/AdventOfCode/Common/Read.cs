using System.Runtime.CompilerServices;

namespace AdventOfCode;

internal static class Read
{
    public static IReader Sample(int? n = null) => new Reader($"sample{n}.txt");

    public static IReader Input() => new Reader("input.txt");
    public static string InputText([CallerFilePath] string path = "") => Input().Text(path);
    public static string[] InputLines([CallerFilePath] string path = "") => Input().Lines(path).ToArray();
    public static Stream InputStream([CallerFilePath] string path = "") => Input().Stream(path);
    public static string SampleText(int? n = null, [CallerFilePath] string path = "") => Sample(n).Text(path);
    public static string[] SampleLines(int? n = null, [CallerFilePath] string path = "") => Sample(n).Lines(path).ToArray();
    public static Stream SampleStream(int? n = null, [CallerFilePath] string path = "") => Sample(n).Stream(path);
    public static string[] String(string s) => ReadLines(s).ToArray();
    private static IEnumerable<string> ReadLines(string s)
    {
        using var sr = new StringReader(s);
        while (sr.Peek() >= 0) yield return sr.ReadLine()!;
    }

    public interface IReader 
    {
        string Text([CallerFilePath] string path = "");
        IEnumerable<string> Lines([CallerFilePath] string path = "");
        Stream Stream([CallerFilePath] string path = "");
    }
    class Reader : IReader
    {
        string _filename;
        public Reader(string filename)
        {
            _filename = filename;
        }
        public string Text(string path) => File.ReadAllText(Path.Combine(Path.GetDirectoryName(path) ?? "", _filename));
        public IEnumerable<string> Lines(string path) => File.ReadLines(Path.Combine(Path.GetDirectoryName(path) ?? "", _filename));
        public Stream Stream(string path) => File.OpenRead(Path.Combine(Path.GetDirectoryName(path) ?? "", _filename));

    }

}


internal static class FileSystem
{
    public static string GetDirectory([CallerFilePath] string path = "") => Path.GetDirectoryName(path)!;
    public static string GetFileName(string filename, [CallerFilePath] string path = "") => Path.Combine(GetDirectory(path), filename);
}