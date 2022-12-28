var catalog = new Catalog();
var sw = Stopwatch.StartNew();
var part1 = Part1Impl();
var part2 = Part2Impl();
Console.WriteLine((part1, part2, sw.Elapsed));
int Part1Impl() => (
    from items in catalog.Scenarios()
    let player = new Player(100, items.Sum(i => i.damage), items.Sum(i => i.armor))
    where Play(player, new Player(104, 8, 1))
    select items.Sum(i => i.cost)).Min();
int Part2Impl() => (
    from items in catalog.Scenarios()
    let player = new Player(100, items.Sum(i => i.damage), items.Sum(i => i.armor))
    where !Play(player, new Player(104, 8, 1))
    select items.Sum(i => i.cost)).Max();
bool Play(Player player, Player boss)
{
    while (true)
    {
        boss = boss.HitBy(player);
        if (boss.hitpoints <= 0)
            return true;
        player = player.HitBy(boss);
        if (player.hitpoints <= 0)
            return false;
    }
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