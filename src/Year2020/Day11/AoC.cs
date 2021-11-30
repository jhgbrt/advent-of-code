namespace AdventOfCode.Year2020.Day11;

public class AoCImpl : AoCBase
{
    static Grid grid = Grid.FromFile("input.txt");

    public override object Part1() => grid.Handle(Grid.Rule1);
    public override object Part2() => grid.Handle(Grid.Rule2);

}