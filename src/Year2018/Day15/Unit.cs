namespace AdventOfCode.Year2018.Day15;

class Unit
{
    public Unit((int x, int y) coordinate, int health, int power)
    {
        Coordinate = coordinate;
        Health = health;
        AttackPower = power;
    }

    public (int x, int y) Coordinate { get; private set; }
    public int Health { get; private set; }
    public int AttackPower { get; }
    public bool IsNeighbour(Unit other) => Math.Abs(Coordinate.x - other.Coordinate.x) + Math.Abs(Coordinate.y - other.Coordinate.y) == 1;

    internal Unit AttackBy(Unit unit)
    {
        Health -= unit.AttackPower;
        return this;
    }

    internal Unit MoveTo((int x, int y) p)
    {
        Coordinate = p;
        return this;
    }
}