namespace AdventOfCode.Year2017.Day21;

class Rule
{
    private readonly char[,] _source;
    private readonly char[,] _result;

    public Rule(string[] source, string[] result)
    {
        _source = source.ToRectangular();
        _result = result.ToRectangular();
    }

    public int Size => _source.GetUpperBound(0) + 1;

    public static Rule Parse(string rule)
    {
        var parts = rule.Split(" => ");
        return new Rule(parts[0].Split('/'), parts[1].Split('/'));
    }

    public char[,] Result => _result;

    public bool IsMatch(string input) => IsMatch(input.ReadLines().ToRectangular());
    public bool IsMatch(char[,] input)
    {
        return input.Transform(0, false).AllEqual(_source)
               || input.Transform(1, false).AllEqual(_source)
               || input.Transform(2, false).AllEqual(_source)
               || input.Transform(3, false).AllEqual(_source)
               || input.Transform(0, true).AllEqual(_source)
               || input.Transform(1, true).AllEqual(_source)
               || input.Transform(2, true).AllEqual(_source)
               || input.Transform(3, true).AllEqual(_source);
    }

    public override string ToString()
    {
        return $"{string.Join("/", _source.FromRectangular())} => {string.Join("/", _result.FromRectangular())}";
    }
}