namespace AdventOfCode.Year2020.Day07;

public class AoC202007
{
    static string key = "shiny gold";

    public object Part1() => relations().ToLookup(x => x.Child).AllParents(key).Select(x => x.Parent).Distinct().Count();
    public object Part2() => relations().ToLookup(x => x.Parent).AllChildren(key, 1).Select(x => x.n* x.item.N).Sum();
    static IEnumerable<Relation> relations() =>
            from line in Read.InputLines()
            let parent = line[0..line.IndexOf(" bags contain ")]
            from substr in line[(parent.Length + 14)..line.Length].Split(", ")
            let n = substr[0..2] switch
            {
                "no" => 0,
                _ => int.Parse(substr[0..substr.IndexOf(" ")])
            }
            let child = substr[2..].Remove(substr[2..].IndexOf(" bag"))
            select new Relation(parent, n, child);

}
record Relation(string Parent, int N, string Child);
