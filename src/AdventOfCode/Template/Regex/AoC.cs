﻿namespace AdventOfCode.YearYYYY.DayDD;
public class AoCYYYYDD
{
    public AoCYYYYDD():this(Read.InputLines(), Console.Out) {}
    readonly TextWriter writer;
    readonly ImmutableArray<Item> items;
    internal IEnumerable<Item> Items => items;
    public AoCYYYYDD(string[] input, TextWriter writer)
    {
        items = input.Select(s => Regexes.MyRegex().As<Item>(s)).ToImmutableArray();
        this.writer = writer;
    }

    public int Part1()
    {
        foreach (var item in items)
            writer.WriteLine(item);

        return -1;
    }
    public int Part2() => -1;
}

readonly record struct Item(string name, int n);

static partial class Regexes
{
    [GeneratedRegex(@"^(?<name>.*): (?<n>\d+)$")]
    public static partial Regex MyRegex();
}

public class AoCYYYYDDTests
{
    private readonly AoCYYYYDD sut;
    public AoCYYYYDDTests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoCYYYYDD(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
        Assert.Equal(2, sut.Items.Count());
        Assert.Equal("foo", sut.Items.First().name);
        Assert.Equal(1, sut.Items.First().n);
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(-1, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(-1, sut.Part2());
    }
}