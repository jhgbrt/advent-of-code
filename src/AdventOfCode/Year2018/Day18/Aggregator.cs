namespace AdventOfCode.Year2018.Day18;

class Aggregator
{
    List<int> Indices = new List<int>();
    List<int> Values = new List<int>();
    public Aggregator Add(int i, Grid g, int v)
    {
        Indices.Add(i);
        Values.Insert(0, v);
        return this;
    }

    public bool HasPattern()
    {
        var max = Values.Count / 2;
        var pattern = (
            from i in Enumerable.Range(10, max)
            where Values.Take(i).SequenceEqual(Values.Skip(i).Take(i))
            select i
            ).FirstOrDefault();
        return pattern > 0;
    }
}