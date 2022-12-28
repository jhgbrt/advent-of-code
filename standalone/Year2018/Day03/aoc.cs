var input = File.ReadAllLines("input.txt");
var regex = new Regex(@"#(?<id>\d+) \@ (?<left>\d+),(?<top>\d+): (?<width>\d+)x(?<height>\d+)", RegexOptions.Compiled);
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
(int left, int top, int width, int height, int id) ToRectangle(string input)
{
    var result = regex.Match(input);
    var left = int.Parse(result.Groups["left"].Value);
    var top = int.Parse(result.Groups["top"].Value);
    var width = int.Parse(result.Groups["width"].Value);
    var height = int.Parse(result.Groups["height"].Value);
    var id = int.Parse(result.Groups["id"].Value);
    return (left, top, width, height, id);
}