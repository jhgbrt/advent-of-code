var input = File.ReadAllLines("input.txt");
var rotate = new Regex("rotate (?<op>(row|column)) (x|y)=(?<i>\\d*) by (?<by>\\d*)", RegexOptions.Compiled);
var rect = new Regex("rect (?<rows>\\d*)x(?<cols>\\d)*", RegexOptions.Compiled);
var sw = Stopwatch.StartNew();
var part1 = Run().Count;
var part2 = Run().ToString();
Console.WriteLine((part1, part2, sw.Elapsed));
Display Run()
{
    var display = new Display(6, 50);
    foreach (var line in input)
    {
        var matchRect = rect.Match(line);
        if (matchRect.Success)
        {
            var rows = int.Parse(matchRect.Groups["rows"].ToString());
            var cols = int.Parse(matchRect.Groups["cols"].ToString());
            display.Rect(rows, cols);
        }

        var matchRotate = rotate.Match(line);
        if (matchRotate.Success)
        {
            var op = matchRotate.Groups["op"].ToString();
            var i = int.Parse(matchRotate.Groups["i"].ToString());
            var by = int.Parse(matchRotate.Groups["by"].ToString());
            if (op == "row")
                display.RotateRow(i, by);
            else
                display.RotateCol(i, by);
        }
    }

    return display;
}