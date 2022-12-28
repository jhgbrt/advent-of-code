var instructions = File.ReadAllLines("input.txt").Select(Instruction.Parse).ToImmutableList();
var sw = Stopwatch.StartNew();
var part1 = new Cpu(instructions).Run().MaxCurrentValue();
var part2 = new Cpu(instructions).Run().MaxValueEver();
Console.WriteLine((part1, part2, sw.Elapsed));