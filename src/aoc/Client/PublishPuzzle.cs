using AdventOfCode.Common;

namespace AdventOfCode.Client;

class PublishPuzzle
{
    public record Options(int year, int day, string? output = null);
    public async Task Run(Options options)
    {
        (var year, var day, var output) = options;
        var dir = AoCLogic.GetDirectory(year, day);

        Console.WriteLine($"Publishing puzzle: {year}/{day}");

        var publishLocation = new DirectoryInfo(output ?? "publish");
        if (!publishLocation.Exists) publishLocation.Create();

        foreach (var extension in new[] {"*.cs", "*.txt", "*.json"})
        foreach (var file in dir.GetFiles(extension))
        {
            Console.WriteLine(file);
            file.CopyTo(Path.Combine(publishLocation.FullName, file.Name));
        }

        var aoc = await File.ReadAllTextAsync(Path.Combine(publishLocation.FullName, "AoC.cs"));
        aoc = aoc
            .Replace($"namespace AdventOfCode.Year{year}.Day{day:00};", "")
            .Replace($"public class AoC{year}{day:00} : AoCBase", "public class AoC")
            .Replace($"typeof(AoC{year}{day:00})", string.Empty)
            .Replace("public override object", "public object");

        await File.WriteAllTextAsync(Path.Combine(publishLocation.FullName, "AoC.cs"), aoc);

        await File.WriteAllTextAsync(Path.Combine(publishLocation.FullName, "Program.cs"), $@"Console.WriteLine(Run(aoc => aoc.Part1()));
Console.WriteLine(Run(aoc => aoc.Part2()));

(T, TimeSpan) Run<T>(Func<AoC, T> f)
{{
    var sw = Stopwatch.StartNew();
    return (f(new AoC()), sw.Elapsed);
}}");

        await File.WriteAllTextAsync(Path.Combine(publishLocation.FullName, "Read.cs"), @"
internal class Read
{
	public static string InputText() => Text(""input.txt"");
	public static string[] InputLines() => Lines(""input.txt"").ToArray();
	public static StreamReader InputStream() => new StreamReader(Stream(""input.txt""));

	public static string Text(string name)
	{
		using var stream = Stream(name);
		using var sr = new StreamReader(stream);
		return sr.ReadToEnd();
	}
	public static IEnumerable<string> Lines(string name)
	{
		using var stream = Stream(name);
		using var sr = new StreamReader(stream);
		{
			while (sr.Peek() >= 0)
				yield return sr.ReadLine()!;
		}
	}

	public static Stream Stream(string name) => File.OpenRead(name);
}
");
        await File.WriteAllTextAsync(Path.Combine(publishLocation.FullName, "aoc.csproj"), @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project Sdk=""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net6.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>
  <ItemGroup>
    <Using Include=""System.Diagnostics"" />
    <Using Include=""System.Reflection"" />
    <Using Include=""System.Text"" />
    <Using Include=""System.Text.Json"" />
    <Using Include=""System.Text.RegularExpressions"" />
    <Using Include=""System.Collections.Immutable"" />
  </ItemGroup>
</Project>");

    }
}



