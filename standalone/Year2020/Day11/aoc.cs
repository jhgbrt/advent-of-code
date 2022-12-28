var grid = Grid.FromFile();
var sw = Stopwatch.StartNew();
var part1 = grid.Handle(Grid.Rule1);
var part2 = grid.Handle(Grid.Rule2);
Console.WriteLine((part1, part2, sw.Elapsed));