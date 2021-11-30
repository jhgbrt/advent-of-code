namespace AdventOfCode.Year2015.Day21;

class Catalog
{
    public IEnumerable<IEnumerable<Item>> Scenarios()
    {
        foreach (var weapon in Weapons)
        {
            yield return new Item[] { weapon };
            foreach (var ring1 in Rings)
            {
                yield return new Item[] { weapon, ring1 };
                foreach (var ring2 in Rings.Where(r => r != ring1))
                    yield return new Item[] { weapon, ring1, ring2 };
            }
            foreach (var armor in Armors)
            {
                yield return new Item[] { weapon, armor };
                foreach (var ring1 in Rings)
                {
                    yield return new Item[] { weapon, ring1, armor };
                    foreach (var ring2 in Rings.Where(r => r != ring1))
                        yield return new Item[] { weapon, armor, ring1, ring2 };
                }
            }
        }
    }
    public Weapon[] Weapons { get; set; } = Array.Empty<Weapon>();
    public Armor[] Armors { get; set; } = Array.Empty<Armor>();
    public Ring[] Rings { get; set; } = Array.Empty<Ring>();
}