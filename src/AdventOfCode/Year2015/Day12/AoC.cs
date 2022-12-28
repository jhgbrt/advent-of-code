namespace AdventOfCode.Year2015.Day12;

public class AoC201512
{
    static string input = Read.InputText();

    public object Part1() => Traverse(Root(), false);

    public object Part2() => Traverse(Root(), true);

    static JsonElement Root() => JsonDocument.Parse("{\"root\": " + input + "}")!.RootElement;

    static int Traverse(JsonElement n, bool removeRed) => n.ValueKind switch
    {
        JsonValueKind.Object when removeRed && n.EnumerateObject().Any(e => e.Value.ValueKind is JsonValueKind.String && e.Value.GetString() == "red") => 0,
        JsonValueKind.Object => n.EnumerateObject().Select(e => Traverse(e.Value, removeRed)).Sum(),
        JsonValueKind.Array => n.EnumerateArray().Select(e => Traverse(e, removeRed)).Sum(),
        JsonValueKind.Number => n.GetInt32(),
        JsonValueKind.String => 0
    };
}