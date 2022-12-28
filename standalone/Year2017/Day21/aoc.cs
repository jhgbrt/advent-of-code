var rules = File.ReadAllLines("input.txt").Select(Rule.Parse).ToArray();
var input = ".#.\r\n..#\r\n###".ReadLines().ToRectangular();
var sw = Stopwatch.StartNew();
var part1 = new ExpandingGrid(input).Expand(rules, 5).Count('#');
var part2 = new ExpandingGrid(input).Expand(rules, 18).Count('#');
Console.WriteLine((part1, part2, sw.Elapsed));