namespace AdventOfCode.Year2017.Day21;

class ExpandingGrid
{
    private readonly char[,] _grid;

    public ExpandingGrid(char[,] grid)
    {
        _grid = grid;
    }

    public ExpandingGrid Expand(Rule[] rules, int times)
    {
        var g = this;
        for (int i = 0; i < times; i++) g = g.Expand(rules);
        return g;
    }
    public ExpandingGrid Expand(Rule[] rules)
    {
        var inputSize = _grid.GetUpperBound(0) + 1;
        var inputSquareSize = inputSize % 2 == 0 ? 2 : 3;
        var pixelsPerSmallSquare = inputSquareSize * inputSquareSize;
        var inputPixels = inputSize * inputSize;
        var nofSquares = inputPixels / pixelsPerSmallSquare;
        var outputSquareSize = inputSquareSize + 1;
        var outputPixels = outputSquareSize * outputSquareSize * nofSquares;

        var q =
            from square in _grid.Squares(inputSquareSize)
            let rule = rules.First(r => r.IsMatch(square))
            select rule.Result;

        var result = q.Assemble(outputSquareSize, (int)Math.Sqrt(outputPixels));
        return new ExpandingGrid(result);
    }

    public int Count(char c)
    {
        return _grid.OfType<char>().Count(x => x == c);
    }

    public override string ToString()
    {
        return string.Join(Environment.NewLine, _grid.FromRectangular());
    }
}