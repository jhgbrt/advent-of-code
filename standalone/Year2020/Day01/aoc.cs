var input = File.ReadAllLines("input.txt");
var numbers = input.Select(int.Parse).ToArray();
var sw = Stopwatch.StartNew();
var part1 = numbers.Part1();
var part2 = numbers.Part2();
Console.WriteLine((part1, part2, sw.Elapsed));
record Pair(int i, int j)
{
    public int Sum => i + j;
}

record Triplet(int i, int j, int k)
{
    public int Sum => i + j + k;
}