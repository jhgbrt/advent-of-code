var input = File.ReadAllText("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Part1(input);
var part2 = Part2(input);
Console.WriteLine((part1, part2, sw.Elapsed));
Node ToTree(string input)
{
    var enumerator = input.ToIntegers().GetEnumerator();
    var root = ReadNode(enumerator);
    return root;
}

Node ReadNode(IEnumerator<int> enumerator)
{
    var nofchildren = enumerator.Next();
    var nofmetadata = enumerator.Next();
    var children = Enumerable.Range(0, nofchildren).Select(i => ReadNode(enumerator)).ToList();
    var metadata = enumerator.Read(nofmetadata).ToList();
    return new Node(children, metadata);
}