using static System.Math;
struct Component
{
    public readonly int Port1;
    public readonly int Port2;
    public Component(int port1, int port2)
    {
        Port1 = port1;
        Port2 = port2;
    }

    public int Smallest => Min(Port1, Port2);
    public bool Matches(int pins) => pins == Port1 || pins == Port2;
    public int Other(int pins) => pins == Port1 ? Port2 : Port1;
    public int Strength => Port1 + Port2;
    public override string ToString() => $"{Port1}/{Port2}";

    public static Component Parse(string s)
    {
        var split = s.Split('/');
        return new Component(int.Parse(split[0]), int.Parse(split[1]));
    }
}