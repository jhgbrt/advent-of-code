namespace AdventOfCode.Year2015.Day15;

public class AoC201515
{
    static string[] input = Read.InputLines();
    static Regex regex = new Regex(@"(?<name>\w+): capacity (?<capacity>[-\d]+), durability (?<durability>[-\d]+), flavor (?<flavor>[-\d]+), texture (?<texture>[-\d]+), calories (?<calories>[-\d]+)");

    static ImmutableList<Ingredient> ingredients = (
        from line in input
        let match = regex.Match(line)
        let name = match.Groups["name"].Value
        let capacity = int.Parse(match.Groups["capacity"].Value)
        let durability = int.Parse(match.Groups["durability"].Value)
        let flavor = int.Parse(match.Groups["flavor"].Value)
        let texture = int.Parse(match.Groups["texture"].Value)
        let calories = int.Parse(match.Groups["calories"].Value)
        select new Ingredient(name, capacity, durability, flavor, texture, calories)
        ).ToImmutableList();

    public object Part1() => (
            from f in Factors()
            let capacity = Math.Max(0, ingredients.Zip(f).Select(i => i.First.capacity * i.Second).Sum())
            let durability = Math.Max(0, ingredients.Zip(f).Select(i => i.First.durability * i.Second).Sum())
            let flavor = Math.Max(0, ingredients.Zip(f).Select(i => i.First.flavor * i.Second).Sum())
            let texture = Math.Max(0, ingredients.Zip(f).Select(i => i.First.texture * i.Second).Sum())
            select capacity * durability * flavor * texture
        ).Max();

    public object Part2() => (
            from f in Factors()
            let capacity = Math.Max(0, ingredients.Zip(f).Select(i => i.First.capacity * i.Second).Sum())
            let durability = Math.Max(0, ingredients.Zip(f).Select(i => i.First.durability * i.Second).Sum())
            let flavor = Math.Max(0, ingredients.Zip(f).Select(i => i.First.flavor * i.Second).Sum())
            let texture = Math.Max(0, ingredients.Zip(f).Select(i => i.First.texture * i.Second).Sum())
            let calories = Math.Max(0, ingredients.Zip(f).Select(i => i.First.calories * i.Second).Sum())
            where calories == 500
            select capacity * durability * flavor * texture
        ).Max();

    static IEnumerable<long[]> Factors()
    {
        for (int i = 0; i <= 100; i++)
            for (int j = 0; j <= 100 - i; j++)
                for (int k = 0; k <= 100 - i - j; k++)
                    if (i + j + k < 100)
                        yield return new[] { i, j, k, 100L - i - j - k };
    }
}
record struct Ingredient(string name, int capacity, int durability, int flavor, int texture, int calories);
