var input = File.ReadAllText("input.txt");
var sw = Stopwatch.StartNew();
var part1 = input.Select(c => c switch
{
    '(' => +1,
    ')' => -1,
    _ => throw new Exception()
}).Sum();
var part2 = Part2();
Console.WriteLine((part1, part2, sw.Elapsed));
object Part2()
{
    var sum = 0;
    for (int i = 0; i < input.Length; i++)
    {
        sum += input[i] switch
        {
            '(' => +1,
            ')' => -1,
            _ => throw new Exception()
        };
        if (sum == -1)
            return i + 1;
    }

    return -1;
}