var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = (
    from line in input
    let half = line.Length / 2
    let part1 = line[0..half]
    let part2 = line[half..]
    from common in part1.Intersect(part2)
    select Priority(common)).Sum();
var part2 = (
    from chunk in input.Chunk(3) from common in chunk[0].Intersect(chunk[1]).Intersect(chunk[2]).Distinct() let priority = Priority(common) select priority).Sum();
Console.WriteLine((part1, part2, sw.Elapsed));
int Priority(char c) => c switch
{
    >= 'a' and <= 'z' => c - 'a' + 1,
    >= 'A' and <= 'Z' => c - 'A' + 27,
    _ => throw new InvalidOperationException("unexpected character")
};