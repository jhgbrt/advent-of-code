using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using static System.Linq.Enumerable;
using Ticket = System.Collections.Immutable.ImmutableArray<int>;
using static AoC;

var (fields, myticket, nearbytickets) = ParseFile("input.txt");

var part1 =(
    from t in nearbytickets
    from value in t
    where !fields.Any(f => f.IsValid(value))
    select value
    ).Sum();

var fieldValuesByIndex = (
    from t in nearbytickets
    from p in t.Select((value, index) => (value, index))
    where fields.Any(f => f.IsValid(p.value))
    select (p.index, p.value)
    ).ToLookup(p => p.index, p => p.value);

var fieldNameIndex = ImmutableArray.Create<(int index, string name)>();
while (fields.Any())
{
    var identifiedFields = 
        from index in Range(0, myticket.Length)
        let candidates =
            from c in fields
            where fieldValuesByIndex[index].All(c.IsValid)
            select c
        where candidates.Count() == 1
        select (index, field: candidates.Single());

    fieldNameIndex = fieldNameIndex.AddRange(identifiedFields.Select(f => (f.index, f.field.Name)));
    fields  = fields.RemoveRange(identifiedFields.Select(f => f.field));
}

var part2 = (
    from f in fieldNameIndex
    let fieldname = f.name
    where fieldname.StartsWith("departure")
    select myticket[f.index]
    ).Aggregate(1L, (x,y) => x*y);


Console.WriteLine((part1, part2));

record FieldDef(string Name, ImmutableHashSet<int> Values) { public bool IsValid(int value) => Values.Contains(value); }

static class AoC
{
    internal static (ImmutableArray<FieldDef> fields, Ticket myticket, ImmutableArray<Ticket> othertickets) ParseFile(string input)
    {
        using var sr = new StreamReader(File.OpenRead(input));

        var fields = (
            from line in sr.ReadWhile(s => !string.IsNullOrEmpty(s))
            let keyvalue = line.Split(":")
            let key = keyvalue[0]
            let ranges = (
                from items in keyvalue[1].Split(" or ").Select(s => s.Split("-"))
                let range = (start:int.Parse(items[0]), end: int.Parse(items[1]))
                from value in Range(range.start, range.end - range.start + 1)
                select value
                ).ToImmutableHashSet()
            select new FieldDef(key, ranges)
            ).ToImmutableArray();

        foreach (var line in sr.ReadWhile(line => line != "your ticket:"));

        var myticket = (
            from line in sr.ReadWhile(s => !string.IsNullOrEmpty(s))
            select line.Split(",").Select(int.Parse).ToImmutableArray()
            ).Single();

        foreach (var line in sr.ReadWhile(line => line != "nearby tickets:")) ;

        var nearbytickets = (
            from line in sr.ReadWhile(s => !string.IsNullOrEmpty(s))
            select line.Split(",").Select(int.Parse).ToImmutableArray()
            ).ToImmutableArray();

        return (fields, myticket, nearbytickets);

    }

    static IEnumerable<string> ReadWhile(this StreamReader sr, Func<string, bool> predicate)
    {
        for (string line = sr.ReadLine(); predicate(line); line = sr.ReadLine())
            yield return line;
    }
}
