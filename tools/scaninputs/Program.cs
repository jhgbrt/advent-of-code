
foreach (var file in Directory.GetFiles(".", "input.txt", SearchOption.AllDirectories))
{
    var lines = File.ReadLines(file).Take(2).ToList();
    if (lines.Count == 1 && lines[0].All(c => char.IsDigit(c) || c == ' '))
    {
        Console.WriteLine(file);
        Console.WriteLine(lines[0]);
    }
}


