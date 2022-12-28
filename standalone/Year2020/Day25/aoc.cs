var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = "";
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    var (key1, key2) = (2084668L, 3704642L);
    long prime = 20201227, value = 1, result = 1;
    while (value != key2)
    {
        (value, result) = (value * 7 % prime, result * key1 % prime);
    }

    return result;
}