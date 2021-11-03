using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.RegularExpressions;

var lines = await File.ReadAllLinesAsync("input.txt");

Part1();
Part2();


void Part1()
{
    var lights = new Dictionary<Coordinate, int>();
    void turnon(Dictionary<Coordinate, int> grid, Coordinate coordinate) => grid[coordinate] = 1;
    void turnoff(Dictionary<Coordinate, int> grid, Coordinate coordinate) => grid[coordinate] = 0;
    void toggle(Dictionary<Coordinate, int> grid, Coordinate coordinate) => grid[coordinate] = grid[coordinate] == 0 ? 1 : 0;

    foreach (var line in lines)
    {
        var instruction = Instruction.Parse(line);
        ApplyInstruction(lights, instruction, turnon, turnoff, toggle);
    }

    Console.WriteLine(lights.Values.Sum());


}

void Part2()
{
    var lights = new Dictionary<Coordinate, int>();

    void turnon(Dictionary<Coordinate, int> grid, Coordinate coordinate) => grid[coordinate] += 1;
    void turnoff(Dictionary<Coordinate, int> grid, Coordinate coordinate) => grid[coordinate] -= 1;
    void toggle(Dictionary<Coordinate, int> grid, Coordinate coordinate) => grid[coordinate] += 2;

    foreach (var line in lines)
    {
        var instruction = Instruction.Parse(line);
        ApplyInstruction(lights, instruction, turnon, turnoff, toggle);
    }

    Console.WriteLine(lights.Values.Sum());

}

void ApplyInstruction(
    Dictionary<Coordinate, int> grid,
    Instruction instruction,
    Action<Dictionary<Coordinate, int>, Coordinate> turnon,
    Action<Dictionary<Coordinate, int>, Coordinate> turnoff,
    Action<Dictionary<Coordinate, int>, Coordinate> toggle
    )
{
    for (var i = instruction.TopLeft.x; i <= instruction.BottomRight.x; i++)
        for (var j = instruction.TopLeft.y; j <= instruction.BottomRight.y; j++)
        {
            if (!grid.ContainsKey(new(i,j))) grid[new(i,j)] = 0;
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


enum InstructionEnum
{
    TurnOn,
    TurnOff,
    Toggle
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
            "toggle" => new Instruction(InstructionEnum.Toggle, topleft, bottomright)
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