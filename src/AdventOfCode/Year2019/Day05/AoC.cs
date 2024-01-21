
namespace AdventOfCode.Year2019.Day05;

public class AoC201905
{
    long[] program;
    public AoC201905() : this(Read.InputLines()) { }
    public AoC201905(string[] input)
    {
        program = input[0].Split(',').Select(long.Parse).ToArray();
    }
 
    public object Part1() => Run(1).Last();
    public object Part2() => Run(5).Last();
    public IEnumerable<long> Run(long input) => new IntCode(program).Run([input]);
  

}

public class Specs
{
    private readonly ITestOutputHelper output;
    public Specs(ITestOutputHelper output)
    {
        this.output = output;
    }

 

    [Fact]
    public void Part1()
    {
        Assert.Equal(4887191L, new AoC201905(Read.SampleLines()).Part1());
    }
    [Fact]
    public void Part2()
    {
        Assert.Equal(3419022L, new AoC201905(Read.SampleLines()).Part2());
    }

   

}

