namespace AdventOfCode.Year2020.Day21;

public class AoC202021 : AoCBase
{
    static string[] input = Read.InputLines(typeof(AoC202021));

    public override object Part1() => Part1Impl();
    public override object Part2() => Part2Impl();


    static Regex regex = new Regex(@"^(?<Ingredients>[^(]+) \(contains (?<Allergens>[^)]+)\)");

    static object Part1Impl()
    {
        var foods = from line in input
                    let match = regex.Match(line)
                    let ingredients = match.Groups["Ingredients"].Value.Split(" ")
                    let allergens = match.Groups["Allergens"].Value.Split(", ")
                    select (ingredients, allergens);

        var list1 = (
            from food in foods
            from allergen in food.allergens
            group food.ingredients by allergen into g
            let ingredients = g.Aggregate((a, b) => a.Intersect(b).OrderBy(i => i).ToArray())
            select (allergen: g.Key, ingredients)
            ).ToImmutableList();

        var list2 = ImmutableList<(string allergen, string ingredient)>.Empty;
        while (list1.Any())
        {
            foreach (var item in list1)
            {
                Console.WriteLine($"{item.allergen}: {string.Join(" ", item.ingredients)}");
                var ingredients = item.ingredients.Except(list2.Select(x => x.ingredient));
                if (ingredients.Count() == 1)
                {
                    list2 = list2.Add((item.allergen, ingredients.Single()));
                    list1 = list1.Remove(item);
                }
            }
        }

        var ingredientsWithAllergens = list2.Select(x => x.ingredient).ToHashSet();

        var part1 = (
            from food in foods
            from ingredient in food.ingredients
            where !ingredientsWithAllergens.Contains(ingredient)
            select ingredient
            ).Count();
        return part1;
    }
    static object Part2Impl()
    {
        var foods = from line in input
                    let match = regex.Match(line)
                    let ingredients = match.Groups["Ingredients"].Value.Split(" ")
                    let allergens = match.Groups["Allergens"].Value.Split(", ")
                    select (ingredients, allergens);

        var list1 = (
            from food in foods
            from allergen in food.allergens
            group food.ingredients by allergen into g
            let ingredients = g.Aggregate((a, b) => a.Intersect(b).OrderBy(i => i).ToArray())
            select (allergen: g.Key, ingredients)
            ).ToImmutableList();

        var list2 = ImmutableList<(string allergen, string ingredient)>.Empty;
        while (list1.Any())
        {
            foreach (var item in list1)
            {
                Console.WriteLine($"{item.allergen}: {string.Join(" ", item.ingredients)}");
                var ingredients = item.ingredients.Except(list2.Select(x => x.ingredient));
                if (ingredients.Count() == 1)
                {
                    list2 = list2.Add((item.allergen, ingredients.Single()));
                    list1 = list1.Remove(item);
                }
            }
        }

        var part2 = string.Join(",", from item in list2
                                     orderby item.allergen
                                     select item.ingredient);
        return part2;

    }



}