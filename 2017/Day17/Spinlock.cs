public class Spinlock
{
    public static (int index, IList<int> buffer) Find(int steps, int maxValue)
    {
        var currentPosition = 0;
        var currentValue = 0;
        var buffer = new List<int>(maxValue + 1) { 0 };
        for (int i = 0; i < maxValue; i++)
        {
            currentPosition = (currentPosition + steps) % buffer.Count;
            currentValue++;
            currentPosition++;
            buffer.Insert(currentPosition, currentValue);
        }
        return (currentPosition,buffer);
    }

    public static int FindFast(int steps, int maxValue)
    {
        var currentPosition = 0;
        var n = 0;
        var result = 0;
        while (n < maxValue)
        {
            var diff = (n - currentPosition) / steps + 1;
            n += diff;
            currentPosition = (currentPosition + diff * (steps + 1) - 1) % n;
            if (currentPosition == 0 && n < maxValue) result = n;
            currentPosition++;
        }
        return result;
    }
}