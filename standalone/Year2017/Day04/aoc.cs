var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = input.Count(IsValidPassword1);
var part2 = input.Count(IsValidPassword2);
Console.WriteLine((part1, part2, sw.Elapsed));
bool IsValidPassword1(string line)
{
    var words = line.Split(' ');
    return words.Length == words.Distinct().Count();
}

bool IsValidPassword2(string line)
{
    var words = line.Split(' ').Select(w => new string(w.OrderBy(c => c).ToArray())).ToArray();
    return words.Length == words.Distinct().Count();
}