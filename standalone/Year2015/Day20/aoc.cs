var input = 34000000;
var sw = Stopwatch.StartNew();
var part1 = Part1Impl();
var part2 = Part2Impl();
Console.WriteLine((part1, part2, sw.Elapsed));
int Part1Impl()
{
    var result =
        from house in Enumerable.Range(1, input)
        let presents = (
            from elve in Factors(house)
            where house % elve == 0
            select elve * 10).Sum()
        where presents >= input
        select (house, presents);
    return result.First(x => x.presents >= input).house;
}

int Part2Impl()
{
    var deliverycount = new Dictionary<int, int>();
    foreach (int house in Enumerable.Range(1, input))
    {
        int presents = 0;
        foreach (var elve in Factors(house))
        {
            if (!deliverycount.ContainsKey(elve))
                deliverycount[elve] = 1;
            else if (deliverycount[elve] <= 50)
                deliverycount[elve]++;
            if (deliverycount[elve] <= 50)
                presents += elve * 11;
        }

        if (presents > input)
            return house;
    }

    return 0;
}

IEnumerable<int> Factors(int n)
{
    for (int i = 1; i <= Math.Sqrt(n); i++)
    {
        if (n % i == 0)
        {
            yield return i;
            if (i != n / i)
            {
                yield return n / i;
            }
        }
    }
}

partial class AoCRegex
{
}