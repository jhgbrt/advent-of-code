using QuickGraph;

namespace AdventOfCode.Year2018.Day24;

public class AoC201824
{
    static Regex AoC201824Regex = new(
        @"(?<units>\d+) units each with (?<hitpoints>\d+) hit points( \((?<weakness_immunities>[^)]+)\))? with an attack that does (?<damagepoints>\d+) (?<damagetype>[^ ]+) damage at initiative (?<initiative>\d+)"
    );

    private static string[] input = Read.SampleLines();

    private static ImmutableArray<Group> groups = 
        (
        from line in input.Skip(1).TakeWhile(s => !string.IsNullOrEmpty(s)).Select((l,i)=>(l,i))
        select Group.From(AoC201824Regex.As<Data>(line.l), line.i+1, GroupType.ImmuneSystem)
        ).Concat(
        from line in input.SkipWhile(s => !string.IsNullOrEmpty(s)).Skip(2).Select((l, i) => (l, i))
        select Group.From(AoC201824Regex.As<Data>(line.l), line.i + 1, GroupType.Infection)
        ).ToImmutableArray();

    public object Part1()
    {
        foreach (var item in groups) Console.WriteLine(item);
        var armies = groups;
        while (armies.Where(a => a.type == GroupType.ImmuneSystem).Any() && armies.Where(a => a.type == GroupType.Infection).Any())
        {
            armies = Fight(armies);
            break;
        }
        

        return -1;
    }
    public object Part2() => "";


    ImmutableArray<Group> Fight(ImmutableArray<Group> groups)
    {

        var g = from x in groups group x by x.type;

        foreach (var parent in g)
        {
            Console.WriteLine(parent.Key);
            foreach (var child in parent)
            {
                Console.WriteLine($" Group {child.id} contains {child.units} units");
            }
        }


        var attacks = SelectTargets(groups).OrderByDescending(a => a.attacker.initiative);


        foreach (var (attacker, target) in attacks)
        {
            Console.WriteLine($"{attacker.type} group {attacker.id} (initiative {attacker.initiative}) attacks defending group {target.id} {target.Damage(attacker)} damage");
        }


        return groups;
    }

    IEnumerable<(Group attacker, Group target)> SelectTargets(IEnumerable<Group> allgroups)
    {
        return from attacker in allgroups
               orderby attacker.EffectivePower descending, attacker.initiative descending
               let targets = (
                   from d in allgroups
                   where d.type != attacker.type
                   orderby d.Damage(attacker) descending, d.EffectivePower descending, d.initiative descending
                   select Write(attacker, d)
                   )
               let target = targets.FirstOrDefault()
               where target.HasValue
               select (attacker, target.Value);
    }

    (Group attacker, Group defender) Attack(Group attacker, Group target)
    {
        var damage = target.Damage(attacker);
        var lost = damage / target.hitpoints;
        var defender = target with { units = target.units - lost };
        return (attacker, defender);
    }

    private Group? Write(Group attacker, Group target)
    {
        Console.WriteLine($"{attacker.type} group {attacker.id} would deal defending group {target.id} {target.Damage(attacker)} damage");
        return target;
    }


}

readonly record struct Data(int units, int hitpoints, string weakness_immunities, int damagepoints, string damagetype, int initiative);

readonly record struct Group(GroupType type, int id, int units, int hitpoints, string[] weaknesses, string[] immunities, int damagepoints, string damagetype, int initiative)
{
    public static Group From(Data data, int id, GroupType type)
    {
        string[] weaknessess = Array.Empty<string>();
        string[] immunities = Array.Empty<string>();

        if (!string.IsNullOrEmpty(data.weakness_immunities))
        {
            var split = data.weakness_immunities.Split(';', StringSplitOptions.TrimEntries);

            var (first, second) = split switch
            {
                { Length: 2 } when split[0].StartsWith("weak to ") => (split[0], split[1]),
                { Length: 2 } when split[0].StartsWith("immune to ") => (split[1], split[0]),
                { Length: 1 } when split[0].StartsWith("weak to ") => (split[0], string.Empty),
                { Length: 1 } when split[0].StartsWith("immune to ") => (string.Empty, split[0])
            };

            weaknessess = string.IsNullOrEmpty(first) ? weaknessess : first[7..].Split(',', StringSplitOptions.TrimEntries);
            immunities = string.IsNullOrEmpty(second) ? immunities : second[9..].Split(',', StringSplitOptions.TrimEntries);

            //(weaknessess, immunities) = split switch
            //{
            //    { Length: 2 } when split[0].StartsWith("weak to ") => (split[0][7..].Split(',', StringSplitOptions.TrimEntries), split[1][9..].Split(",", StringSplitOptions.TrimEntries)),
            //    { Length: 2 } when split[0].StartsWith("immune to ") => (split[1][7..].Split(',', StringSplitOptions.TrimEntries), split[0][9..].Split(",", StringSplitOptions.TrimEntries)),
            //    { Length: 1 } when split[0].StartsWith("weak to ") => (split[0][7..].Split(',', StringSplitOptions.TrimEntries), immunities),
            //    { Length: 1 } when split[0].StartsWith("immune to ") => (weaknessess, split[0][9..].Split(",", StringSplitOptions.TrimEntries)),
            //    _ => throw new NotSupportedException()
            //};
        }

        return new Group(type, id, data.units, data.hitpoints, weaknessess, immunities, data.damagepoints, data.damagetype, data.initiative);
    }
    public int EffectivePower => units * damagepoints;
    public int Damage(Group other)
    {
        if (weaknesses.Contains(other.damagetype)) return other.units * other.damagepoints * 2;
        if (immunities.Contains(other.damagetype)) return 0;
        return other.units * other.damagepoints;
    }
    public Group Attack(Group attacker) => this with { units = units - Damage(attacker) / hitpoints };

    public override string ToString()
    {
        return $"{type} - {units} units each with {hitpoints} (weaknesses: {string.Join(',', weaknesses)}, immune to {string.Join(',', immunities)}) with an attack that does {damagepoints} {damagetype} damage at initiative {initiative}";
    }
}

enum GroupType
{
    ImmuneSystem,
    Infection
}