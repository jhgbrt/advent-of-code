var input = "ugkcyxxp";
var sw = Stopwatch.StartNew();
var part1 = new Cracker().GeneratePassword1(input, 8);
var part2 = new Cracker().GeneratePassword2(input, 8);
Console.WriteLine((part1, part2, sw.Elapsed));
partial class AoCRegex
{
}