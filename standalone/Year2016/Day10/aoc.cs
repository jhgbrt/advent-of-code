var input = File.ReadAllLines("input.txt").Where(s => !string.IsNullOrEmpty(s)).ToArray();
var re1 = new Regex(@"bot (?<id>\d+) gives low to (?<ltype>bot|output) (?<low>\d+) and high to (?<htype>bot|output) (?<high>\d+)");
var re2 = new Regex(@"value (?<value>\d+) goes to bot (?<destination>\d+)");
var sw = Stopwatch.StartNew();
var part1 = RunInstructions().part1;
var part2 = RunInstructions().part2;
Console.WriteLine((part1, part2, sw.Elapsed));
(int part1, int part2) RunInstructions()
{
    int part1 = 0;
    List<object> instructions = new();
    foreach (var line in input)
    {
        var match1 = re1.Match(line);
        if (match1.Success)
        {
            var id = int.Parse(match1.Groups["id"].Value);
            var low = int.Parse(match1.Groups["low"].Value);
            var ltype = match1.Groups["ltype"].Value;
            var high = int.Parse(match1.Groups["high"].Value);
            var htype = match1.Groups["htype"].Value;
            instructions.Add(new BotInstruction(id, low, ltype, high, htype));
        }
        else
        {
            var match2 = re2.Match(line);
            var value = int.Parse(match2.Groups["value"].Value);
            var destination = int.Parse(match2.Groups["destination"].Value);
            instructions.Add(new ValueInstruction(value, destination));
        }
    }

    Dictionary<int, Bot> bots = Enumerable.Empty<int>().Concat(instructions.OfType<BotInstruction>().Select(i => i.id)).Concat(instructions.OfType<BotInstruction>().Where(i => i.ltype == "bot").Select(i => i.low)).Concat(instructions.OfType<BotInstruction>().Where(i => i.htype == "bot").Select(i => i.high)).Distinct().Select(i => new Bot(i, null, null)).ToDictionary(b => b.id);
    Dictionary<int, int> outputs = Enumerable.Empty<int>().Concat(instructions.OfType<BotInstruction>().Where(i => i.ltype == "output").Select(i => i.low)).Concat(instructions.OfType<BotInstruction>().Where(i => i.htype == "output").Select(i => i.high)).Distinct().ToDictionary(x => x, x => 0);
    HashSet<object> done = new();
    while (done.Count < instructions.Count)
        foreach (var o in instructions)
        {
            if (done.Contains(o))
                continue;
            if (o is ValueInstruction i)
            {
                bots[i.destination] = bots[i.destination].WithValue(i.value);
                done.Add(o);
                continue;
            }
            else if (o is BotInstruction j)
            {
                var bot = bots[j.id];
                if (!bot.Low.HasValue || !bot.High.HasValue)
                    continue;
                if (j.ltype == "bot")
                    bots[j.low] = bots[j.low].WithValue(bot.Low.Value);
                else
                    outputs[j.low] = bot.Low.Value;
                if (j.htype == "bot")
                    bots[j.high] = bots[j.high].WithValue(bot.High.Value);
                else
                    outputs[j.high] = bot.High.Value;
                if (bot.Low == 17 && bot.High == 61)
                    part1 = bot.id;
                done.Add(o);
            }
        }

    return (part1, outputs[0] * outputs[1] * outputs[2]);
}

record BotInstruction(int id, int low, string ltype, int high, string htype);
record ValueInstruction(int value, int destination);
record Value(int value, int destionation);
record struct Bot(int id, int? v1, int? v2)
{
    public int NofValues => (v1.HasValue ? 1 : 0) + (v2.HasValue ? 1 : 0);
    public int? High => NofValues == 2 ? Math.Max(v1 ?? 0, v2 ?? 0) : null;
    public int? Low => NofValues == 2 ? Math.Min(v1 ?? 0, v2 ?? 0) : null;
    public Bot WithValue(int v)
    {
        if (v2.HasValue)
            throw new Exception();
        if (v1.HasValue)
            return this with { v2 = v };
        return this with { v1 = v };
    }
}

;