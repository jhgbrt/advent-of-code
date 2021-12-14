namespace AdventOfCode.Year2020.Day11;

public class AoC202011
{
    static Grid grid = Grid.FromFile("input.txt");

    public object Part1() => grid.Handle(Grid.Rule1);
    public object Part2() => grid.Handle(Grid.Rule2);

}