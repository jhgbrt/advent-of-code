namespace AdventOfCode.Year2017.Day18;

public class AoCImpl : AoCBase
{
    static string[] instructions = Read.InputLines(typeof(AoCImpl));
    public override object Part1() => new CPU1().Load(instructions).Run();
    public override object Part2() => Part2(instructions);

    private static int Part2(string[] instructions)
    {
        var collection1 = new Queue<long>();
        var collection2 = new Queue<long>();
        var cpu0 = new CPU2(0, collection1, collection2);
        var cpu1 = new CPU2(1, collection2, collection1);
        cpu0.Load(instructions);
        cpu1.Load(instructions);
        do
        {
            cpu1.Run();
            cpu0.Run();
        } while (collection1.Any() || collection2.Any());
        return cpu1.Sent;
    }

}