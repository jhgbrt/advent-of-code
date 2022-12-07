using Sprache;

using System.IO;

namespace AdventOfCode.Year2022.Day07;

public class AoC202207
{
    static string[] input = Read.InputLines();

    public object Part1() => Directory.Parse(input).AllChildren().Where(d => d.Size < 100000).Sum(d => d.Size);



    public object Part2()
    {
        var root = Directory.Parse(input);
        var total = 70000000L;
        var required = 30000000L;
        var taken = root.Size;
        var unused = total - taken;
        var tofree = required - unused;
        var todelete = root
            .AllChildren()
            .Where(d => d.Size >= tofree)
            .OrderBy(d => d.Size).First();
        return todelete.Size;
    }
}

abstract class FileSystemEntry
{
    public string Name { get; init; }
    public List<FileSystemEntry> Children { get; } = new();
    public abstract long Size { get; }
}
class Directory : FileSystemEntry
{
    static Regex FileRegex = new(@"^(?<size>\d+) (?<name>.+)$");
    static Regex DirectoryRegex = new(@"^dir (?<name>.+)$");
    public static Directory Parse(string[] input)
    {
        var root = new Directory("/", null);
        var cd = root;
        foreach (var line in input.Skip(1))
        {
            cd = line[0..4] switch
            {
                "$ cd" => cd.Find(line[5..]),
                "$ ls" => cd,
                _ when DirectoryRegex.Match(line) is { Success: true } m
                    => cd.AddDirectory(m.Groups["name"].Value),
                _ when FileRegex.Match(line) is { Success: true } m
                    => cd.AddFile(m.Groups["name"].Value, long.Parse(m.Groups["size"].Value)),
                _ => throw new NotImplementedException()
            };
        }
        return root;
    }
    public IEnumerable<Directory> AllChildren()
    {
        yield return this;
        foreach (var child in Children.OfType<Directory>())
        {
            foreach (var d in child.AllChildren())
                yield return d;
        }
    }
    private Directory(string name, Directory? parent)
    {
        Name = name;
        Parent = parent;
    }
    public Directory? Parent { get; }
    public override long Size => Children.Sum(f => f.Size);
    public Directory AddFile(string name, long size)
    {
        Children.Add(new File(name, size));
        return this;
    }

    public Directory AddDirectory(string name)
    {
        Children.Add(new Directory(name, this));
        return this;;
    }

    public Directory Find(string name) => name switch
    {
        ".." => Parent ?? throw new Exception("root does not have parent"),
        _ => Children.OfType<Directory>().First(c => c.Name == name)
    };

    public override string ToString() => $"dir {Name}";

}
class File : FileSystemEntry
{
    public File(string name, long size)
    {
        Name = name;
        Size = size;
    }
    public override long Size { get; }
    public override string ToString() => $"{Size} {Name}";
}