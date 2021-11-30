namespace AdventOfCode.Year2018.Day19;

public class Specs
{
    string[] testinput = new[]
    {
            @"#ip 0",
            @"seti 5 0 1",
            @"seti 6 0 2",
            @"addi 0 1 0",
            @"addr 1 2 3",
            @"setr 1 0 0",
            @"seti 8 0 4",
            @"seti 9 0 5"
        };



    [Fact]
    public void RunTest()
    {
        var cpu = new CPU(0, testinput.GetInstructions(), new[] { 0L, 0, 0, 0, 0, 0 });
        cpu.Run();
        Assert.Equal(new long[] { 6, 5, 6, 0, 0, 9 }, cpu.Registers);
    }


    [Fact]
    public void GetOpCodeByNameTest()
    {
        var registers = new[] { 0L, 0, 0, 0, 0, 0 };
        var result = OpCode.apply(registers, ("seti", 5, 0, 1));
        Assert.Equal(new[] { 0L, 5, 0, 0, 0, 0 }, result);
    }

    [Fact]
    public void TestPart1()
    {
        var result = AoCImpl.Part1(testinput);
        Assert.Equal(6, result);
    }

    [Fact]
    public void TestPart2()
    {
        var result = AoCImpl.Part2(testinput);
        Assert.Equal(11106760, result);
    }

    [Fact]
    public void addr_adds_register_0_and_register_1()
    {
        var input = new[] { 5L, 3, 1, 0 };
        OpCode.addr(input, 1, 2, 3);
        Assert.Equal(new[] { 5L, 3, 1, 4 }, input);
    }
    [Fact]
    public void addi_adds_register_0_and_value_()
    {
        var input = new[] { 5L, 3, 1, 0 };
        OpCode.addi(input, 1, 2, 3);
        Assert.Equal(new[] { 5L, 3, 1, 5 }, input);
    }

    [Fact]
    public void Test()
    {
        var instructions = Read.InputLines(typeof(AoCImpl)).GetInstructions();
        var result = new CPU(1, instructions, new[] { 0L }).RunReverseEngineered();
        Assert.Equal(11106760, result.A);
    }
}
