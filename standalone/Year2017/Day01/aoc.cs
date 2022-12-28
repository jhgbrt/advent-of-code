var input = File.ReadAllLines("input.txt").First();
var sw = Stopwatch.StartNew();
var part1 = Captcha.Calculate(input, 1);
var part2 = Captcha.Calculate(input, input.Length / 2);
Console.WriteLine((part1, part2, sw.Elapsed));