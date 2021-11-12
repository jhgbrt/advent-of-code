using System.Collections.Immutable;

using static AoC;

Console.WriteLine(Part1());
Console.WriteLine(Part2());

partial class AoC
{
    static ImmutableArray<Spell> AllSpells = new Spell[]
    {
        new("Magic Missile", 53, 4, 0, 0, 0, 1),
        new("Drain", 73, 2, 2, 0, 0, 1),
        new("Shield", 113, 0, 0, 7, 0, 6),
        new("Poison", 173, 3, 0, 0, 0, 6),
        new("Recharge", 229, 0, 0, 0, 101, 5)
    }.ToImmutableArray();

    static string[] input = File.ReadAllLines("input.txt");

    internal static Result Part1() => Run(() => Play(new Player(50, 0, 0, 500), new Player(58, 9, 0, 0), false));
    internal static Result Part2() => Run(() => Play(new Player(50, 0, 0, 500), new Player(58, 9, 0, 0), true));

    static int Play(Player player, Player boss, bool hardMode, bool playerTurn = true, int best = 999999, int spent = 0, ImmutableList<Spell>? activeSpells = null)
    {
        activeSpells ??= ImmutableList<Spell>.Empty;
        if (spent >= best) return best;

        player = player with
        {
            Mana = player.Mana + activeSpells.Sum(s => s.Mana),
            Damage = activeSpells.Sum(s => s.Damage),
            Armor = activeSpells.Sum(s => s.Armor),
            HitPoints = hardMode ? player.HitPoints - 1 : player.HitPoints
        };
        if (player.Lost) return best;

        boss = boss with { HitPoints = boss.HitPoints - player.Damage };
        if (boss.Lost) return spent;

        activeSpells = activeSpells.Select(x => x with { Duration = x.Duration - 1 }).Where(x => x.Duration > 0).ToImmutableList();

        if (playerTurn)
        {
            var candidateSpells = AllSpells.Where(s => !activeSpells.Select(x => x.Name).Contains(s.Name) && s.Cost <= player.Mana);
            if (!candidateSpells.Any()) return best;

            return candidateSpells.Aggregate(best, (b, s) => Math.Min(b, Play(player with
            {
                HitPoints = player.HitPoints + (s.Duration == 1 ? s.Heal : 0),
                Mana = player.Mana - s.Cost
            }, boss, hardMode, false, b, spent + s.Cost, activeSpells.Add(s)))
            );
        }
        else // boss turn
        {
            var damage = Math.Max(boss.Damage - player.Armor, 1);
            player = player with { HitPoints = player.HitPoints - damage };
            return player.Lost ? best : Play(player, boss, hardMode, true, best, spent, activeSpells);
        }
    }
}

public class Tests
{
    [Fact]
    public void Test1() => Assert.Equal(1269, Part1().Value);
    [Fact]
    public void Test2() => Assert.Equal(1309, Part2().Value);
}

record struct Player(int HitPoints, int Damage, int Armor, int Mana)
{
    public bool Lost => HitPoints <= 0;
}

record struct Spell(string Name, int Cost, int Damage, int Heal, int Armor, int Mana, int Duration);