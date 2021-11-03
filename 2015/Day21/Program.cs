
var catalog = new Catalog
{
    Weapons = new Weapon[]
    {
        new(8, 4, 0),
        new(10, 5, 0),
        new(25, 6, 0),
        new(40, 7, 0),
        new(74, 8, 0),
    },
    Armors = new Armor[]
    {
        new(13,0,1),
        new(31,0,2),
        new(53,0,3),
        new(75,0,4),
        new(102,0,5)
    },
    Rings = new Ring[]
    {
        new(25,1,0),
        new(50,2,0),
        new(100,3,0),
        new(20,0,1),
        new(40,0,2),
        new(80,0,3)
    }
};


Console.WriteLine(Part1());
Console.WriteLine(Part2());

int Part1() => (
        from items in catalog.Scenarios()
        let player = new Player(100, items.Sum(i => i.damage), items.Sum(i => i.armor))
        where Play(player, new Player(104, 8, 1))
        select items.Sum(i => i.cost)
    ).Min();



int Part2() => (
        from items in catalog.Scenarios()
        let player = new Player(100, items.Sum(i => i.damage), items.Sum(i => i.armor))
        where !Play(player, new Player(104, 8, 1))
        select items.Sum(i => i.cost)
    ).Max();

bool Play(Player player, Player boss)
{
    while (true)
    {
        boss = boss.HitBy(player);
        if (boss.hitpoints <= 0) return true;
        player = player.HitBy(boss);
        if (player.hitpoints <= 0) return false;
    }
}


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
    public Weapon[] Weapons { get; set; }
    public Armor[] Armors { get; set; }
    public Ring[] Rings { get; set; }
}

record Item(int cost, int damage, int armor);
record Weapon(int cost, int damage, int armor) : Item(cost, damage, armor);
record Armor(int cost, int damage, int armor) : Item(cost, damage, armor)
{
    public static Armor None = new(0, 0, 0);
}
record Ring(int cost, int damage, int armor) : Item(cost, damage, armor)
{
    public static Ring None = new(0, 0, 0);
}
record Player(int hitpoints, int damage, int armor)
{
    public Player HitBy(Player other) => this with { hitpoints = hitpoints - (other.damage - armor) }; 
}