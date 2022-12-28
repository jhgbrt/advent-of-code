var input = File.ReadAllText("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Read.InputText().EncodeToSomethingSimpler().CalculateChecksum();
var part2 = "";
Console.WriteLine((part1, part2, sw.Elapsed));