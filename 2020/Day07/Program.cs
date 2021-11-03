using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

Func<string, IEnumerable<Relation>> relations = (string input) =>
        from line in File.ReadLines(input)
        let parent = line[0..line.IndexOf(" bags contain ")]
        from substr in  line[(parent.Length+14)..line.Length].Split(", ")
        let n = substr[0..2] switch
        {
            "no" => 0,
            _ => int.Parse(substr[0..substr.IndexOf(" ")])
        }
        let child = substr[2..].Remove(substr[2..].IndexOf(" bag"))
        select new Relation(parent, n, child);

var key = "shiny gold";

Func<string, int> Part1 = (string input)
    => relations(input).ToLookup(x => x.Child).AllParents(key).Select(x => x.Parent).Distinct().Count();
Func<string, int> Part2 = (string input)
    => relations(input).ToLookup(x => x.Parent).AllChildren(key, 1).Select(x => x.n * x.item.N).Sum();

Debug.Assert(Part1("example.txt") == 4);
Debug.Assert(Part2("example.txt") == 32);
Debug.Assert(Part2("example2.txt") == 126);

var result1 = Part1("input.txt");
var result2 = Part2("input.txt");
Console.WriteLine((result1, result2));

record Relation(string Parent, int N, string Child);
static class AoC
{
    public static IEnumerable<Relation> AllParents(this ILookup<string, Relation> tree, string child)
    {
        var parents = new [] {child}.SelectMany(c => tree[c]);
        while (parents.Any())
        {
            foreach (var parent in parents) yield return parent;
            parents = parents.Select(p => p.Parent).SelectMany(c => tree[c]);
        }
    }
    public static IEnumerable<(int n, Relation item)> AllChildren(this ILookup<string, Relation> tree, string name, int n)
    {
        foreach (var child in new [] {name}.SelectMany(c => tree[c]))
        {
            yield return (n, child);
            foreach (var item in AllChildren(tree, child.Child, n*child.N)) yield return item;
        }
    }
}
