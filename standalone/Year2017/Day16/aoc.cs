var dancer = new Dancer(new StreamReader(File.OpenRead("input.txt")));
var initial = "abcdefghijklmnop";
var sw = Stopwatch.StartNew();
var part1 = dancer.Run(initial);
var part2 = dancer.Run(initial, 1000000000);
Console.WriteLine((part1, part2, sw.Elapsed));