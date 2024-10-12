namespace AdventOfCode.Year2019.Day14;
public class AoC201914
{
    static string[] input = Read.InputLines();
    public static ImmutableDictionary<string, Reaction> GetReactions(IEnumerable<string> input) => (
        from line in input
        let split = line.Split(" => ", StringSplitOptions.TrimEntries)
        let inputs = from item in split[0].Split(",", StringSplitOptions.TrimEntries)
                     select Regexes.MyRegex().As<Item>(item)
        let output = Regexes.MyRegex().As<Item>(split[1])
        select new Reaction(inputs.ToImmutableArray().WithValueSemantics(), output)
        ).ToImmutableDictionary(x => x.output.name);

    public object Test1()
    {
        var reactions = GetReactions(Read.Sample(1).Lines());
        return GetRequiredOre(reactions);
    }
    public object Test2()
    {
        var reactions = GetReactions(Read.Sample(2).Lines());
        return GetRequiredOre(reactions);
    }
    public object Test3()
    {
        var reactions = GetReactions(Read.Sample(3).Lines());
        return GetRequiredOre(reactions);
    }
    public object Test4()
    {
        var reactions = GetReactions(Read.Sample(4).Lines());
        return GetRequiredOre(reactions);
    }

    public object Part1() => GetRequiredOre(GetReactions(input));


    private static long GetRequiredOre(IReadOnlyDictionary<string, Reaction> reactions, long amount = 1)
    {
        var supply = new Dictionary<string, long>();

        Queue<Item> ingredients = new();
        ingredients.Enqueue(new Item(amount, "FUEL"));
        var ore = 0L;
        while (ingredients.Any())
        {
            var ingredient = ingredients.Dequeue();
            if (ingredient.name == "ORE")
            {
                ore += ingredient.quantity;
            }
            else if (ingredient.quantity < supply.GetValueOrDefault(ingredient.name))
            {
                supply[ingredient.name] = supply.GetValueOrDefault(ingredient.name) - ingredient.quantity;
            }
            else
            {
                var required = ingredient.quantity - supply.GetValueOrDefault(ingredient.name);
                var reaction = reactions[ingredient.name];
                var n = (int)Ceiling(1.0 * required / reaction.output.quantity);
                foreach (var input in reaction.inputs)
                {
                    ingredients.Enqueue(input with { quantity = input.quantity * n });
                }
                var remaining = n * reaction.output.quantity - required;
                supply[ingredient.name] = remaining;
            }
        }
        return ore;
    }

    public long Part2()
    {
        var reactions = GetReactions(input);
        const long total = 1000000000000;
        long guess = 1;
        long ore;

        do
        {
            guess *= 2;
            ore = GetRequiredOre(reactions, guess);
        } while (ore < total);

        var max = guess;
        var min = guess / 2;
        while (min + 1 < max)
        {
            guess = (min+max)/2;
            ore = GetRequiredOre(reactions, guess);
            if (ore > total)
            {
                max = guess;
            }
            else
            {
                min = guess;
            }
        }
        return min;

    }
}

public readonly record struct Item(long quantity, string name) : IComparable<Item>
{
    public int CompareTo(Item other)
    {
        var result = quantity.CompareTo(other.quantity);
        return result switch
        {
            0 => name.CompareTo(other.name),
            _ => result
        };
    }

    public override string ToString() => $"{quantity} {name}";
}
public readonly record struct Reaction(ValueList<Item> inputs, Item output)
{
    public override string ToString() => $"{inputs} => {output}";
}

static partial class Regexes
{
    [GeneratedRegex(@"^(?<quantity>\d+) (?<name>[A-Z]+)$")]
    public static partial Regex MyRegex();
}