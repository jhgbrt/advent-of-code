using System.Text.RegularExpressions;

Regex regex = new Regex(@"(?<name>\w+) can fly (?<speed>\d+) km/s for (?<fly>\d+) seconds, but then must rest for (?<rest>\d+) seconds");

bool test = false;
int maxtime;
string filename;
if (!test)
{
    maxtime = 2503;
    filename = "input.txt";
}
else
{
    maxtime = 1000;
    filename = "sample.txt";
}

var lines = File.ReadAllLines(filename);

var entries = (
    from line in lines
    let match = regex.Match(line)
    let name = match.Groups["name"].Value
    let speed = int.Parse(match.Groups["speed"].Value)
    let fly = int.Parse(match.Groups["fly"].Value)
    let rest = int.Parse(match.Groups["rest"].Value)
    select new Entry(name, speed, fly, rest)
    );

// part1
Console.WriteLine(entries.Select(e => e.GetDistance(maxtime)).Max());

// part2
var points = entries.ToDictionary(e => e, e => 0);
var tick = entries.Select(e => (entry: e, distance: 0)).ToList();
for (int t = 1; t <= maxtime; t++)
{
    tick = (
        from e in tick
        select (e.entry, e.entry.GetDistance(t))
        ).ToList();

    var winners = (
        from e in tick
        group e by e.distance into g
        orderby g.Key descending
        select g
        ).First();

    foreach (var winner in winners)
        points[winner.entry] += 1;

    if (test)
    {
        foreach (var item in tick)
            Console.WriteLine($"{t} - {item} - {points[item.entry]}");
        Console.WriteLine();
    }

}

Console.WriteLine(points.MaxBy(x => x.Value));       

record Entry(string name, int speed, int fly, int rest)
{
    public int GetDistance(int time)
    {
        var remainder = time % (fly + rest);
        var nofIntervals = time / (fly + rest);
        var totalFlyTime = nofIntervals * fly + Math.Min(fly, remainder);
        var distance = totalFlyTime * speed;
        return distance;
    }
}