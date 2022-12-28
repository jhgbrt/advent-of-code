var input = File.ReadAllText("input.txt");
var sw = Stopwatch.StartNew();
var part1 = CheckSum.CheckSum1(new StringReader(input));
var part2 = CheckSum.CheckSum2(new StringReader(input));
Console.WriteLine((part1, part2, sw.Elapsed));