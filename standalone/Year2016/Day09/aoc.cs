var input = File.ReadAllLines("input.txt").First();
var sw = Stopwatch.StartNew();
var part1 = input.GetDecompressedSize(0);
var part2 = input.GetDecompressedSize2(0, input.Length);
Console.WriteLine((part1, part2, sw.Elapsed));
class AoCRegex
{
}