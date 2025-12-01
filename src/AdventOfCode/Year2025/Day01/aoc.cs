using Microsoft.Build.Framework;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using Sprache;

using System.Linq;

namespace AdventOfCode.Year2025.Day01;

record struct Instruction(char direction, int distance);

public class AoC202501(string[] input, TextWriter writer)
{
    public AoC202501() : this(Read.InputLines(), Console.Out) 
    {
    }

    Instruction[] instructions = input
        .Select(line => new Instruction(line[0], int.Parse(line[1..])))
        .ToArray();

    public int Part1()
    {
        var value = 50;
        var password = 0;
        foreach (var instr in instructions)
        {
            value = Mod(instr.direction switch
            {
                'L' => value - instr.distance,
                _ => value + instr.distance
            }, 100);

            if (value == 0) password++;
        }
        return password; 
    }
    
    private static int Mod(int value, int mod)
    {
        var result = value % mod;
        return result < 0 ? result + mod : result;
    }


    public int Part2() => -1;
}

public class AoC202501Tests
{
    private readonly AoC202501 sut;
    public AoC202501Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202501(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Instruction[] instructions = Read.SampleLines()
            .Select(line => new Instruction(line[0], int.Parse(line[1..])))
            .ToArray();
        var result = instructions.Aggregate(50, (value, instr) =>
        Mod(instr.direction switch
        {
            'L' => value - instr.distance,
            _ => value + instr.distance
        }, 100) == 0 ? 1 : 0);

        Assert.Equal(3, result);
    }
    private static int Mod(int value, int mod)
    {
        var result = value % mod;
        return result < 0 ? result + mod : result;
    }
    [Fact]
    public void TestPart1()
    {
        Assert.Equal(3, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(-1, sut.Part2());
    }
}