namespace AdventOfCode.Year2019.Day13;
public class AoC201913
{
    internal static string[] input = Read.InputLines();

    long[] program = input.First().Split(',').Select(long.Parse).ToArray();

    public object Part1() => (from chunk in new IntCode(program).Run().Chunked3()
                              let x = chunk.a
                              let y = chunk.b
                              let id = chunk.c
                              where id == 2
                              select id).Count();
    public object Part2()
    {
        program[0] = 2;
        var cpu = new IntCode(program);

        var (paddle, ball, score) = (0L, 0L, 0L);
        
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
                (-1, 0, var id) => (paddle, ball, id),
                (var x, _, 3L) => (x, ball, score),
                (var x, _, 4L) => (paddle, x, score),
                _ => (paddle, ball, score)
            };
           
        }

        return score;
    }
    private static (long a, long b, long c) Step(IntCode cpu, long input) => (cpu.Run(input), cpu.Run(input), cpu.Run(input));
}

enum Direction
{
    Left = -1,
    None = 0,
    Right = 1
}
