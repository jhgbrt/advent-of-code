namespace AdventOfCode.Year2016.Day08;

public class AoC201608 : AoCBase
{
    
    public static string[] input = Read.InputLines(typeof(AoC201608));

    public override object Part1() => Run().Count;
    public override object Part2() => Run().ToString();

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