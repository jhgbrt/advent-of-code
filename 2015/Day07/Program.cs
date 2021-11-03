
using System.Text.RegularExpressions;



var v1 = Part1();
Console.WriteLine(v1);
var v2 = Part2(v1);
Console.WriteLine(v2);


static int Part1()
{
    var nodes = BuildNodes();
    Resolve(nodes);
    var a = nodes["a"];
    var v = a.GetValue();
    return v;
}

static int Part2(int v1)
{
    var nodes = BuildNodes();
    nodes["b"] = (LiteralValueNode)nodes["b"] with { LiteralValue = v1 };
    Resolve(nodes);
    var a = nodes["a"];
    var v = a.GetValue();
    return v;
}


static Dictionary<string, Node> BuildNodes()
{
    var lines = File.ReadAllLines("input.txt");

    var regexes = new (Regex r, Func<Match, Node> f)[]
    {
    (new (@"^(?<value>[\d]+) -> (?<name>[a-z]+)$"), m => new LiteralValueNode(m.Groups["name"].Value, int.Parse(m.Groups["value"].Value))),
    (new (@"^(?<value>[a-z]+) -> (?<name>[a-z]+)$"), m => new ConnectorNode(m.Groups["name"].Value, m.Groups["value"].Value)),
    (new (@"^NOT (?<value>[a-z]+) -> (?<name>[a-z]+)$"), m => new NotNode(m.Groups["name"].Value, m.Groups["value"].Value)),
    (new (@"^(?<left>[a-z]+) AND (?<right>[a-z]+) -> (?<name>[a-z]+)$"), m => new AndNode(m.Groups["name"].Value, m.Groups["left"].Value, m.Groups["right"].Value)),
    (new (@"^(?<left>[\d]+) AND (?<right>[a-z]+) -> (?<name>[a-z]+)$"), m => new AndValueNode(m.Groups["name"].Value, int.Parse(m.Groups["left"].Value), m.Groups["right"].Value)),
    (new (@"^(?<left>[a-z]+) OR (?<right>[a-z]+) -> (?<name>[a-z]+)$"), m => new OrNode(m.Groups["name"].Value, m.Groups["left"].Value, m.Groups["right"].Value)),
    (new (@"^(?<operand>[a-z]+) LSHIFT (?<value>\d+) -> (?<name>[a-z]+)$"), m => new LeftShiftNode(m.Groups["name"].Value, m.Groups["operand"].Value, int.Parse(m.Groups["value"].Value))),
    (new (@"^(?<operand>[a-z]+) RSHIFT (?<value>\d+) -> (?<name>[a-z]+)$"), m => new RightShiftNode(m.Groups["name"].Value, m.Groups["operand"].Value, int.Parse(m.Groups["value"].Value)))
    };

    var nodes = new Dictionary<string, Node>();

    foreach (var line in lines)
    {
        try
        {
            var match = regexes.Select(x => (m: x.r.Match(line), x.f)).First(x => x.m.Success);
            var node = match.f(match.m);
            nodes[node.Name] = node;
        }
        catch
        {
            Console.WriteLine($"not recognized: {line}");
        }
    }

    return nodes;
}

static void Resolve(Dictionary<string, Node> nodes)
{
    foreach (var node in nodes.Values)
    {
        node.Resolve(nodes);
    }
}

abstract record Node(string Name)
{
    int? _value;
    static int indent;
    public int GetValue()
    {
        if (!_value.HasValue)
        {
            _value = GetValueImpl();
        }
        return _value.Value;
    }

    protected abstract int GetValueImpl();
    public abstract void Resolve(IReadOnlyDictionary<string, Node> nodes);
}

record LiteralValueNode(string Name, int LiteralValue) : Node(Name)
{
    protected override int GetValueImpl() => LiteralValue;
    public override void Resolve(IReadOnlyDictionary<string, Node> nodes) {}
    public override string ToString() => $"{LiteralValue} -> {Name}";
}

record ConnectorNode(string Name, string SourceName) : Node(Name)
{
    public Node? Source { get; private set; }

    protected override int GetValueImpl() => Source?.GetValue() ?? 0;

    public override void Resolve(IReadOnlyDictionary<string, Node> nodes)  { Source = nodes[SourceName]; }
    public override string ToString() => $"{SourceName} -> {Name}";
}

record AndNode(string Name, string Left, string Right) : Node(Name)
{
    public Node? LeftOperand { get; private set; }
    public Node? RightOperand { get; private set; }

    protected override int GetValueImpl()
    {
        if (LeftOperand == null || RightOperand == null) return 0;
        var left = LeftOperand.GetValue();
        var right = RightOperand.GetValue();
        return left & right;
    }

    public override void Resolve(IReadOnlyDictionary<string, Node> nodes) { LeftOperand = nodes[Left]; RightOperand = nodes[Right]; }
  
    public override string ToString() => $"{Left} AND {Right} -> {Name}";
}
record AndValueNode(string Name, int Value, string RightOperandName) : Node(Name)
{
    public Node? RightOperand { get; private set; }

    protected override int GetValueImpl()
    {
        if (RightOperand == null) return 0;
        var right = RightOperand.GetValue();
        return Value & right;
    }

    public override void Resolve(IReadOnlyDictionary<string, Node> nodes) { RightOperand = nodes[RightOperandName]; }
   
    public override string ToString() => $"{Value} AND {RightOperandName} -> {Name}";
}

record OrNode(string Name, string LeftOperandName, string RightOperandName) : Node(Name)
{
    public Node? LeftOperand { get; private set; }
    public Node? RightOperand { get; private set; }

    protected override int GetValueImpl()
    {
        if (LeftOperand == null || RightOperand == null) return 0;
        var left = LeftOperand.GetValue();
        var right = RightOperand.GetValue();
        return left | right;
    }

    public override void Resolve(IReadOnlyDictionary<string, Node> nodes) { LeftOperand = nodes[LeftOperandName]; RightOperand = nodes[RightOperandName]; }
   
    public override string ToString() => $"{LeftOperandName} OR {RightOperandName} -> {Name}";
}
record NotNode(string Name, string OperandName) : Node(Name)
{
    public Node? Operand { get; private set; }

    protected override int GetValueImpl() => ~(Operand?.GetValue() ?? 0);

    public override void Resolve(IReadOnlyDictionary<string, Node> nodes) { Operand = nodes[OperandName]; }
    public override string ToString() => $"NOT {OperandName} -> {Name}";
}

record LeftShiftNode(string Name, string OperandName, int ShiftValue) : Node(Name)
{
    public Node? Operand { get; private set; }

    protected override int GetValueImpl() => (Operand?.GetValue() ?? 0) << ShiftValue;

    public override void Resolve(IReadOnlyDictionary<string, Node> nodes) { Operand = nodes[OperandName]; }
    public override string ToString() => $"{OperandName} LSHIFT {ShiftValue} -> {Name}";
}
record RightShiftNode(string Name, string OperandName, int ShiftValue) : Node(Name)
{
    public Node? Operand { get; private set; }

    protected override int GetValueImpl() => (Operand?.GetValue() ?? 0) >> ShiftValue;

    public override void Resolve(IReadOnlyDictionary<string, Node> nodes) { Operand = nodes[OperandName]; }
  
    public override string ToString() => $"{OperandName} RSHIFT {ShiftValue} -> {Name}";
}

