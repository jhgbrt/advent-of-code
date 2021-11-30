namespace AdventOfCode.Year2018.Day03;

public class AoC201803 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC201803));

    public override object Part1() => Part1(input);
    public override object Part2() => Part2(input);

    public static int Part1(string[] input)
    {
        var query = from line in input
                    select ToRectangle(line) into r
                    from x in Enumerable.Range(r.left, r.width)
                    from y in Enumerable.Range(r.top, r.height)
                    group r by (x, y) into g
                    where g.Count() > 1
                    select g;

        return query.Count();
    }

    public static int Part2(string[] data)
    {

        var rectangles = data.Select(ToRectangle).ToList();

        var query = from r in rectangles
                    from x in Enumerable.Range(r.left, r.width)
                    from y in Enumerable.Range(r.top, r.height)
                    group r by (x, y) into g
                    where g.Count() > 1
                    from r in g
                    select r.id;

        var overlapping = new HashSet<int>(query);
        var single = rectangles.Single(r => !overlapping.Contains(r.id));
        return single.id;
    }

    private static readonly Regex regex = new Regex(@"#(?<id>\d+) \@ (?<left>\d+),(?<top>\d+): (?<width>\d+)x(?<height>\d+)", RegexOptions.Compiled);
    public static (int left, int top, int width, int height, int id) ToRectangle(string input)
    {
        var result = regex.Match(input);
        var left = int.Parse(result.Groups["left"].Value);
        var top = int.Parse(result.Groups["top"].Value);
        var width = int.Parse(result.Groups["width"].Value);
        var height = int.Parse(result.Groups["height"].Value);
        var id = int.Parse(result.Groups["id"].Value);
        return (left, top, width, height, id);
    }
}