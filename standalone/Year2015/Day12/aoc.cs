var input = File.ReadAllText("input.txt");
var sw = Stopwatch.StartNew();
var part1 = Traverse(Root(), false);
var part2 = Traverse(Root(), true);
Console.WriteLine((part1, part2, sw.Elapsed));
JsonElement Root() => JsonDocument.Parse("{\"root\": " + input + "}")!.RootElement;
int Traverse(JsonElement n, bool removeRed) => n.ValueKind switch
{
    JsonValueKind.Object when removeRed && n.EnumerateObject().Any(e => e.Value.ValueKind is JsonValueKind.String && e.Value.GetString() == "red") => 0,
    JsonValueKind.Object => n.EnumerateObject().Select(e => Traverse(e.Value, removeRed)).Sum(),
    JsonValueKind.Array => n.EnumerateArray().Select(e => Traverse(e, removeRed)).Sum(),
    JsonValueKind.Number => n.GetInt32(),
    JsonValueKind.String => 0
};