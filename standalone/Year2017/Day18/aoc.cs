var instructions = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = new CPU1().Load(instructions).Run();
var part2 = Part2(instructions);
Console.WriteLine((part1, part2, sw.Elapsed));