var input = File.ReadAllLines("input.txt");
var bots = (
    from line in input
    let bot = Bot.TryParse(line)
    where bot.HasValue
    select bot.Value).ToArray();
var strongest = bots.MaxBy(b => b.r);
var sw = Stopwatch.StartNew();
var part1 = Part1();
var part2 = "";
Console.WriteLine((part1, part2, sw.Elapsed));
object Part1()
{
    return string.Empty;
    // TODO this is not correct for some reason
    //return bots.Count(strongest.InRange);
}

readonly record struct Bot(Pos position, int r)
{
    static Regex regex = new Regex(@"pos=<(?<X>\d+),(?<Y>\d+),(?<Z>\d+)>, r=(?<r>\d+)", RegexOptions.Compiled);
    public static Bot? TryParse(string s)
    {
        var match = regex.Match(s);
        return match.Success ? new Bot(new(int.Parse(match.Groups["X"].Value), int.Parse(match.Groups["Y"].Value), int.Parse(match.Groups["Z"].Value)), int.Parse(match.Groups["r"].Value)) : null;
    }

    public bool InRange(Bot other) => other.position.Distance(position) <= r;
}

readonly record struct Pos(int X, int Y, int Z)
{
    public int Distance(Pos other) => Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z);
}