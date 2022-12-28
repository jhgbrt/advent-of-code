var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Chunks().Select(chunk => chunk.Select(int.Parse).Sum()).Max();
var part2 = Chunks().Select(chunk => chunk.Select(int.Parse).Sum()).OrderDescending().Take(3).Sum();
Console.WriteLine((part1, part2, sw.Elapsed));
IEnumerable<IEnumerable<string>> Chunks()
{
    List<string> chunk = new();
    foreach (var line in input)
    {
        if (string.IsNullOrEmpty(line))
        {
            yield return chunk;
            chunk.Clear();
        }
        else
        {
            chunk.Add(line);
        }
    }

    if (chunk.Any())
        yield return chunk;
}