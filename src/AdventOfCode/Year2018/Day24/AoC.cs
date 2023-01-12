namespace AdventOfCode.Year2018.Day24;

public class AoC201824
{
    private static Regex AoC201824Regex = Regexes.AoC201824Regex();
    private static Regex NotNormalized = Regexes.NotNormalized();
    private static string[] input = Read.InputLines().Select(Normalize).ToArray();

    private static string Normalize(string line)
    {
        var match = NotNormalized.Match(line);
        if (!match.Success) return line;
        return NotNormalized.Replace(line, "$1$4$3$2$5");
    }

    private static ImmutableArray<Group> Groups() =>
        (
            from line in input.Skip(1).TakeWhile(s => !string.IsNullOrEmpty(s)).Select((l, i) => (l, i))
            select AoC201824Regex.As<Group>(line.l, new { id = line.i + 1, type = GroupType.ImmuneSystem })
        ).Concat(
            from line in input.SkipWhile(s => !string.IsNullOrEmpty(s)).Skip(2).Select((l, i) => (l, i))
            select AoC201824Regex.As<Group>(line.l, new { id = line.i + 1, type = GroupType.Infection })
        ).ToImmutableArray();

    public object Part1() => DoFights(Groups(), 0).units;
    public object Part2() 
    {
        var l = 0;
        var h = int.MaxValue / 2;
        while (h - l > 1)
        {
            var m = (h + l) / 2;
            var (winner, _) = DoFights(Groups(), m);
            if (winner == GroupType.ImmuneSystem)
                h = m;
            else
                l = m;
        }
        return DoFights(Groups(), h).units;
    }
    
    (GroupType? winner, int units) DoFights(ImmutableArray<Group> list, int boost)
    {
        var armies = (from item in list
                      select item.type == GroupType.ImmuneSystem ? item.Boost(boost) : item).ToList();

        while (armies.Any(a => a.type == GroupType.ImmuneSystem)
            && armies.Any(a => a.type == GroupType.Infection))
        {
            var units = armies.Sum(a => a.units);
            armies = Fight(armies).ToList();
            if (armies.Sum(a => a.units) == units)
                return (null, units);
        }
        return (armies.Select(a => a.type).First(), armies.Sum(x => x.units));
    }


    IEnumerable<Group> Fight(List<Group> army)
    {
        var remainingTargets = army.ToHashSet();

        var targets =
             from attacker in army
             orderby attacker.EffectivePower descending, attacker.initiative descending
             let target = SelectTarget(attacker, remainingTargets)
             where target != null && target.WouldLoseUnits(attacker)
             orderby attacker.initiative descending
             select (attacker, target);

        foreach (var (attacker,target) in targets)
        {
            var damage = target.DamageFrom(attacker);
            target.TakeHit(damage);
        }

        return army.Where(g => g.units > 0).ToImmutableArray();
    }
    Group? SelectTarget(Group attacker, HashSet<Group> possibleTargets)
    {
        var max = possibleTargets.Max(target => target.DamageFrom(attacker));
        if (max == 0) return default;

        var selected = (
            from target in possibleTargets
            where target.type != attacker.type
            && target.DamageFrom(attacker) == max
            orderby target.EffectivePower descending, target.initiative descending
            select target
            ).First();

        possibleTargets.Remove(selected);

        return selected;
    }
}

class Group
{
    public Group(GroupType type, int id, int units, int hitpoints, string[] weaknesses, string[] immunities, int damagepoints, string damagetype, int initiative)
    {
        this.type = type;
        this.id = id;
        this.units = units;
        this.hitpoints = hitpoints;
        this.weaknesses = weaknesses;
        this.immunities = immunities;
        this.damagepoints = damagepoints;
        this.damagetype = damagetype;
        this.initiative = initiative;
    }
    int id;
    string[] weaknesses;
    string[] immunities;
    int damagepoints;
    string damagetype;

    public int hitpoints { get; }
    public int initiative { get; }
    public GroupType type { get; }
    public int units { get; private set; }
    public int EffectivePower => units * damagepoints;
    public int DamageFrom(Group other)
    {
        if (type == other.type) return 0;
        if (weaknesses.Contains(other.damagetype)) return other.EffectivePower * 2;
        if (immunities.Contains(other.damagetype)) return 0;
        return other.EffectivePower;
    }

    public bool WouldLoseUnits(Group other) => DamageFrom(other) / hitpoints > 0;

    public bool TakeHit(int damage)
    {
        var loss = damage / hitpoints;
        units = Max(0, units - loss);
        return loss > 0;
    }
    public Group Boost(int boost)
    {
        damagepoints += boost;
        return this;
    }


    public override string ToString() => $"{type} - {units} units each with {hitpoints} (weaknesses: {string.Join(',', weaknesses)}, immune to {string.Join(',', immunities)}) with an attack that does {damagepoints} {damagetype} damage at initiative {initiative}";
}

enum GroupType
{
    ImmuneSystem,
    Infection
}

static partial class Regexes
{

    [GeneratedRegex(@"^(?<units>\d+) units each with (?<hitpoints>\d+) hit points( \((weak to (?<weaknesses>[^;]+))?(; )?(immune to (?<immunities>[^)]+))?\))? with an attack that does (?<damagepoints>\d+) (?<damagetype>[^ ]+) damage at initiative (?<initiative>\d+)$")]
    public static partial Regex AoC201824Regex();
    [GeneratedRegex("([^(]*\\()(immune to[^;]+)(; )(weak to [^)]+)(\\).*)")]
    public static partial Regex NotNormalized();
}
