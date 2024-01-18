namespace AdventOfCode.Year2019.Day13;
public class AoC201913
{
    internal static string[] input = Read.InputLines();
    public AoC201913() : this(Read.InputLines()){ }
    public AoC201913(string[] input)
    {
        program = input.First().Split(',').Select(long.Parse).ToArray();
    }

    long[] program;

    public object Part1() => (from chunk in new IntCode(program).Run().Chunked3()
                              let x = chunk.a
                              let y = chunk.b
                              let id = chunk.c
                              where id == 2
                              select id).Count();
    public long Part2()
    {
        program[0] = 2;
        var cpu = new IntCode(program);

        (long paddle, long ball, long score) = (0, 0, 0);
        
        while (!cpu.IsTerminated)
        {
            var input = (paddle - ball) switch
            {
                > 0 => -1,
                < 0 => 1,
                0 => 0
            };
            
            (paddle, ball, score) = Step(cpu, input) switch
            {
                (-1, 0, long id) => (paddle, ball, id),
                (long x, _, 3) => (x, ball, score),
                (long x, _, 4) => (paddle, x, score),
                _ => (paddle, ball, score)
            };
           
        }

        return score;
    }
     static (long? a, long? b, long? c) Step(IntCode cpu, int input) => (cpu.Run(input), cpu.Run(input) , cpu.Run(input));
}

enum Direction
{
    Left = -1,
    None = 0,
    Right = 1
}

public class Tests
{
    [Fact]
    public void Part1()
    {
        var sut = new AoC201913(Read.SampleLines());
        Assert.Equal(380, sut.Part1());
    }
    [Fact]
    public void Part2()
    {
        var sut = new AoC201913(Read.SampleLines());
        Assert.Equal(18647L, sut.Part2());
    }
}

