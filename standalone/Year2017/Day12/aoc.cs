var input = File.ReadAllLines("input.txt");
var edges = (
    from line in input
    let parts = line.Split("<->").Select(s => s.Trim()).ToArray()
    let vertex1 = int.Parse(parts[0])
    from vertex2 in parts[1].Split(',').Select(int.Parse)
    select (vertex1: vertex1, vertex2: vertex2)).ToArray();
var sw = Stopwatch.StartNew();
var part1 = new Graph(edges).Count(0);
var part2 = new Graph(edges).SubGraphs().Count;
Console.WriteLine((part1, part2, sw.Elapsed));