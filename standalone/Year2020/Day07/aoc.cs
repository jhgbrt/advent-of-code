var key = "shiny gold";
var sw = Stopwatch.StartNew();
var part1 = relations().ToLookup(x => x.Child).AllParents(key).Select(x => x.Parent).Distinct().Count();
var part2 = relations().ToLookup(x => x.Parent).AllChildren(key, 1).Select(x => x.n * x.item.N).Sum();
Console.WriteLine((part1, part2, sw.Elapsed));
IEnumerable<Relation> relations() =>
    from line in File.ReadAllLines("input.txt")
    let parent = line[0..line.IndexOf(" bags contain ")]
    from substr in line[(parent.Length + 14)..line.Length].Split(", ")
    let n = substr[0..2] switch
    {
        "no" => 0,
        _ => int.Parse(substr[0..substr.IndexOf(" ")])
    }
    let child = substr[2..].Remove(substr[2..].IndexOf(" bag"))
    select new Relation(parent, n, child);
record Relation(string Parent, int N, string Child);