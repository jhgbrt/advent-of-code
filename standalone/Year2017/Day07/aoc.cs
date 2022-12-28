var sw = Stopwatch.StartNew();
var part1 = Tree.Parse(File.ReadAllText("input.txt")).Root.Label;
var part2 = Tree.Parse(Read.InputText()).FindInvalidNode().RebalancingWeight;
Console.WriteLine((part1, part2, sw.Elapsed));