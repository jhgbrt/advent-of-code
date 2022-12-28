var input = File.ReadAllLines("input.txt");
var sw = Stopwatch.StartNew();
var part1 = HexGrid.Calculate(input.SelectMany(l => l.Split(',')).ToArray()).distance;
var part2 = HexGrid.Calculate(input.SelectMany(l => l.Split(',')).ToArray()).max;
Console.WriteLine((part1, part2, sw.Elapsed));