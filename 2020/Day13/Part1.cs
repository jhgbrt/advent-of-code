namespace AdventOfCode.Year2020.Day13;

class Part1
{
    public static int Run()
    {
        var example = @"939
7,13,x,x,59,x,31,19";

        var input = @"1000434
17,x,x,x,x,x,x,41,x,x,x,x,x,x,x,x,x,983,x,29,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,x,19,x,x,x,23,x,x,x,x,x,x,x,397,x,x,x,x,x,37,x,x,x,x,x,x,13";

        using var r = new StringReader(input);
        var start = int.Parse(r.ReadLine());

        var query = from x in r.ReadLine().Split(",")
                    where x != "x"
                    let id = int.Parse(x)
                    let timestamp = EnumerableEx.InfiniteSequence(start).First(t => t % id == 0)
                    orderby timestamp ascending
                    select (id, timestamp);
        var result = query.First();

        return (result.id * (result.timestamp - start));

    }
}

static class EnumerableEx
{
    public static IEnumerable<int> InfiniteSequence(int start)
    {
        while (true) yield return start++;
    }
}
