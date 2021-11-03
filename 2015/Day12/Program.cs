using Newtonsoft.Json.Linq;
var input = File.ReadAllText("input.txt");
var jobject = JObject.Parse("{\"root\": " + input + "}");
var root = jobject["root"];

var result1 = Traverse(root, false);
var result2 = Traverse(root, true);
Console.WriteLine((result1, result2));

int Traverse(JToken o, bool removeRed) => o switch
{
    JObject when removeRed && o.Children().OfType<JProperty>().Any(p => p.Children().OfType<JValue>().Any(v => v.Value<string>() == "red")) => 0,
    JValue v when int.TryParse(v.Value<string>(), out var i) => i,
    JValue => 0,
    _ => o.Children().Select(x => Traverse(x, removeRed)).Sum(),
};

