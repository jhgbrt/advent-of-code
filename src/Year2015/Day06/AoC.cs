namespace AdventOfCode.Year2015.Day06;

public class AoCImpl : AoCBase
{
    static IReadOnlyCollection<string> lines = Read.Lines(typeof(AoCImpl), "input.txt").ToList();
    public override object Part1() => Run(
            (grid, c) => grid[c.x, c.y] = 1,
            (grid, c) => grid[c.x, c.y] = 0,
            (grid, c) => grid[c.x, c.y] = grid[c.x, c.y] == 0 ? 1 : 0);

    public override object Part2() => Run(
            (grid, c) => grid[c.x, c.y] += 1,
            (grid, c) => grid[c.x, c.y] = Math.Max(0, grid[c.x, c.y] - 1),
            (grid, c) => grid[c.x, c.y] = grid[c.x, c.y] += 2);

    static object Run(
        Action<int[,], Coordinate> turnon,
        Action<int[,], Coordinate> turnoff,
        Action<int[,], Coordinate> toggle
        )
    {
        var lights = new int[1000, 1000];
        foreach (var line in lines)
        {
            var instruction = Instruction.Parse(line);
            ApplyInstruction(lights, instruction, turnon, turnoff, toggle);
        }
        return Sum(lights);
    }
    static int Sum(int[,] lights)
    {
        var sum = 0;
        for (int x = 0; x < 1000; x++)
            for (int y = 0; y < 1000; y++)
                sum += lights[x, y];
        return sum;
    }

    static void ApplyInstruction(
        int[,] grid,
        Instruction instruction,
        Action<int[,], Coordinate> turnon,
        Action<int[,], Coordinate> turnoff,
        Action<int[,], Coordinate> toggle
        )
    {
        for (var i = instruction.TopLeft.x; i <= instruction.BottomRight.x; i++)
            for (var j = instruction.TopLeft.y; j <= instruction.BottomRight.y; j++)
            {
                switch (instruction.WhatToDo)
                {
                    case InstructionEnum.TurnOn:
                        turnon(grid, new(i, j));
                        break;
                    case InstructionEnum.TurnOff:
                        turnoff(grid, new(i, j));
                        break;
                    case InstructionEnum.Toggle:
                        toggle(grid, new(i, j));
                        break;
                }
            }
    }

}
readonly record struct Instruction(InstructionEnum WhatToDo, Coordinate TopLeft, Coordinate BottomRight)
{
    static Regex _regex = new Regex(@"(?<instruction>turn on|turn off|toggle) (?<topleft>[0-9]+,[0-9]+) through (?<bottomright>[0-9]+,[0-9]+)", RegexOptions.Compiled);

    public static Instruction Parse(string s)
    {
        var match = _regex.Match(s);
        if (!match.Success) throw new Exception("invalid input");
        var topleft = Coordinate.Parse(match.Groups["topleft"].Value);
        var bottomright = Coordinate.Parse(match.Groups["bottomright"].Value);
        return match.Groups["instruction"].Value switch
        {
            "turn on" => new Instruction(InstructionEnum.TurnOn, topleft, bottomright),
            "turn off" => new Instruction(InstructionEnum.TurnOff, topleft, bottomright),
            "toggle" => new Instruction(InstructionEnum.Toggle, topleft, bottomright),
            _ => throw new Exception()
        };
    }
}
readonly record struct Coordinate(int x, int y)
{
    static Regex _regex = new Regex(@"(?<x>[0-9]+),(?<y>[0-9]+)", RegexOptions.Compiled);
    public static Coordinate Parse(string s)
    {
        var match = _regex.Match(s);
        if (!match.Success) throw new Exception("invalid input");
        return new Coordinate(int.Parse(match.Groups["x"].Value), int.Parse(match.Groups["y"].Value));
    }

}

enum InstructionEnum
{
    TurnOn,
    TurnOff,
    Toggle
}
