namespace AdventOfCode.Year2019.Day13;
public class AoC201913
{
    internal static string[] input = Read.InputLines();

    ImmutableDictionary<int, int> program = input.First().Split(',').Select(int.Parse).Select((n, i) => (n, i: (int)i)).ToImmutableDictionary(x => x.i, x => x.n);

    public object Part1() => (from chunk in new IntCode(program).Run().Chunked3()
                              let x = chunk.a
                              let y = chunk.b
                              let id = chunk.c
                              where id == 2
                              select id).Count();
    public object Part2()
    {
        program = program.SetItem(0, 2);
        var cpu = new IntCode(program);

        var (paddle, ball, score) = (0, 0, 0);
        
        while (!cpu.IsTerminated)
        {
            var input = (paddle - ball) switch
            {
                > 0 => -1,
                < 0 => 1,
                0 => 0
            };
            
            (paddle, ball, score) = cpu.Step(input) switch
            {
                (-1, 0, int id) => (paddle, ball, id),
                (int x, _, 3) => (x, ball, score),
                (int x, _, 4) => (paddle, x, score),
                _ => (paddle, ball, score)
            };
           
        }

        return score;
    }
}

enum Direction
{
    Left = -1,
    None = 0,
    Right = 1
}
