using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var regex = new Regex(@"^(?<Ingredients>[^(]+) \(contains (?<Allergens>[^)]+)\)");
var foods = from line in File.ReadLines(@"..\input.txt")
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

var part2 = string.Join(",", from item in list2
                             orderby item.allergen
                             select item.ingredient);

Console.WriteLine((part1, part2));

