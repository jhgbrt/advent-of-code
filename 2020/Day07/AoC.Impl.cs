namespace AdventOfCode.Year2020.Day07;

partial class AoC
{
    static string key = "shiny gold";

    internal static Result Part1() => Run(() => Part1("input.txt"));
    internal static Result Part2() => Run(() => Part2("input.txt"));
    static IEnumerable<Relation> relations(string input) =>
            from line in File.ReadLines(input)
            let parent = line[0..line.IndexOf(" bags contain ")]
            from substr in line[(parent.Length + 14)..line.Length].Split(", ")
            let n = substr[0..2] switch
            {
                "no" => 0,
                _ => int.Parse(substr[0..substr.IndexOf(" ")])
            }
            let child = substr[2..].Remove(substr[2..].IndexOf(" bag"))
            select new Relation(parent, n, child);

    internal static int Part1(string input)
        => relations(input).ToLookup(x => x.Child).AllParents(key).Select(x => x.Parent).Distinct().Count();
    internal static int Part2(string input)
        => relations(input).ToLookup(x => x.Parent).AllChildren(key, 1).Select(x => x.n * x.item.N).Sum();

}
record Relation(string Parent, int N, string Child);
