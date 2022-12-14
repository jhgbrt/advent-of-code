namespace AdventOfCode.Year2022.Day14;
public class AoC202214
{
    static string[] sample = Read.SampleLines();
    static string[] input = Read.InputLines();

    (int x, int y)[] lines = (from line in input
                              from part in line.Split(" -> ")
                              let coordinates = part.Split(",")
                              let x = int.Parse(coordinates[0])
                              let y = int.Parse(coordinates[1])
                              select (x, y)).ToArray();

    public int Part1() => Grid.Parse(input).Simulate();
    public int Part2() => Grid.Parse(sample).Simulate2();
}


