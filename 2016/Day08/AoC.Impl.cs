namespace AdventOfCode.Year2016.Day08;

partial class AoC
{
    static bool test = false;
    public static string[] input = File.ReadAllLines(test ? "sample.txt" : "input.txt");

    internal static Result Part1() => Run(() => Run().Count);
    internal static Result Part2() => Run(() => Run().ToString());

    static Regex rotate = new Regex("rotate (?<op>(row|column)) (x|y)=(?<i>\\d*) by (?<by>\\d*)", RegexOptions.Compiled);
    static Regex rect = new Regex("rect (?<rows>\\d*)x(?<cols>\\d)*", RegexOptions.Compiled);
    static Display Run()
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
}