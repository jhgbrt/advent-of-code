namespace AdventOfCode.Year2025.Day10;

// [.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}

// represent target state as an integer, with bits for each position
// represent buttons as arrays of integers, each with bits for positions they toggle

record Machine(int nofIndicators, int targetState, int[] buttons, int[] joltage)
{
    static Range FindRange(ReadOnlySpan<char> input, char start, char end)
    {
        int startIndex = input.IndexOf(start);
        int endIndex = input.LastIndexOf(end) + 1;
        return startIndex..endIndex;
    }
 
    public int Start()
    {
        var combinations = 1 << buttons.Length;
        var minimum = int.MaxValue;
        for (int mask = 0; mask < combinations; mask++)
        {
            int presses = BitOperations.PopCount((uint)mask);
            if (presses >= minimum) continue;

            var state = 0;
            for (int b = 0; b < buttons.Length; b++)
            {
                if ((mask & (1 << b)) == 0) continue;
                state ^= buttons[b];
            }

            if (state == targetState)
            {
                minimum = presses;
            }
        }
        return minimum == int.MaxValue ? -1 : minimum;
    }

    public static Machine Parse(ReadOnlySpan<char> input)
    {
        T[] parseBetween<T>(ReadOnlySpan<char> input, char start, char end, char separator, Func<ReadOnlySpan<char>, T> convert) 
        {
            var startIndex = input.IndexOf(start);
            var endIndex = input.LastIndexOf(end) + 1;
            var span = input[startIndex..endIndex][1..^1];
            var count = span.Count(separator) + 1;
            var ranges = new Range[count];
            span.Split(ranges, separator);
            T[] result = new T[count];
            for (int i = 0; i < count; i++)
            {
                var part = span[ranges[i]];
                result[i] = convert(part);
            }
            return result;
        }


        var startStateEnd = input.IndexOf(']');
        var stateSpan = input[FindRange(input, '[', ']')][1..^1];
        var nofIndicators = stateSpan.Length;
        var targetState = 0;
        for (int i = 0; i < stateSpan.Length; i++)
        {
            if (stateSpan[i] == '#')
            {
                targetState |= (1 << i);
            }
        }

        var buttonsSpan = input[FindRange(input, '(', ')')];
        var numberOfButtons = buttonsSpan.Count(' ') + 1;
        int[] buttons = new int[numberOfButtons];
        Range[] buttonsRanges = new Range[numberOfButtons];
        buttonsSpan.Split(buttonsRanges, ' ');
        for (int i = 0; i < numberOfButtons; i++)
        {
            int button = 0;
            var digitsSeparatedByComma = buttonsSpan[buttonsRanges[i]][1..^1];
            for (int j = 0; j < digitsSeparatedByComma.Length; j += 2)
            {
                var digit = digitsSeparatedByComma[j];
                button |= (1 << (digit - '0'));
            }
            buttons[i] = button;
        }

        int[] joltage = parseBetween<int>(input, '{', '}', ',', span => int.Parse(span));

        return new Machine(nofIndicators, targetState, buttons, joltage);
    }
}
public class AoC202510(string[] input, TextWriter writer)
{
    public AoC202510() : this(Read.InputLines(), Console.Out) {}

    public int Part1()
    {
        var machines = input.Select(s => Machine.Parse(s)).ToArray();
        return machines.Sum(m => m.Start());
    }
    public int Part2()
    {
        return -1;
    }
}

public class AoC202510Tests
{
    private readonly AoC202510 sut;
    public AoC202510Tests(ITestOutputHelper output)
    {
        var input = Read.SampleLines();
        sut = new AoC202510(input, new TestWriter(output));
    }

    [Fact]
    public void TestParsing()
    {
    }

    [Fact]
    public void TestPart1()
    {
        Assert.Equal(7, sut.Part1());
    }

    [Fact]
    public void TestPart2()
    {
        Assert.Equal(33, sut.Part2());
    }

    [Fact]
    public void Machine_Parse_Works()
    {
        var input = "[.##.] (3) (1,3) (0) (9) (0,2) (0,1) {3,5,4,7}";
        var machine = Machine.Parse(input);
        Assert.Equal(0b0110, machine.targetState);
        Assert.Equal([0b1000, 0b1010, 0b0001, 0b1000000000, 0b101, 0b11], machine.buttons);
    }

    [Theory]
    [InlineData("[.##.] (3) (1,3) (2) (2,3) (0,2) (0,1) {3,5,4,7}", 2)]
    [InlineData("[...#.] (0,2,3,4) (2,3) (0,4) (0,1,2) (1,2,3,4) {7,5,12,7,2}", 3)]
    [InlineData("[.###.#] (0,1,2,3,4) (0,3,4) (0,1,2,4,5) (1,2) {10,11,11,5,10,5}", 2)]
    public void Machine_Start_Works(string input, int expected)
    {
        var machine = Machine.Parse(input);
        var result = machine.Start();
        Assert.Equal(expected, result);
    }
}