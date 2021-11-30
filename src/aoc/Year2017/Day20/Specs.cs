namespace AdventOfCode.Year2017.Day20;

// pos(t) = p + vt + t(t+1)/2*a
public class Specs
{
    string input = "p=< 1,2,3>, v=< 2,3,4>, a=<-1,1,2>";

    [Fact]
    public void Particle_Parse()
    {
        var particle = Particle.Parse(input);
        Assert.Equal((1, 2, 3), particle.Position);
        Assert.Equal((2, 3, 4), particle.Velocity);
        Assert.Equal((-1, 1, 2), particle.Acceleration);
    }
    [Fact]
    public void Particle_Tick()
    {
        var particle = Particle.Parse(input);
        particle = particle.Tick();
        Assert.Equal((-1, 1, 2), particle.Acceleration);
        Assert.Equal((1, 4, 6), particle.Velocity);
        Assert.Equal((2, 6, 9), particle.Position);
    }

    [Fact]
    public void Test()
    {
        var g = new[]
        {
            (1, 2, 3),
            (1, 2, 3),
        }.GroupBy(x => x);
        Assert.Single(g);
    }

}

