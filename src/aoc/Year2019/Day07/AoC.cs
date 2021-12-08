using AdventOfCode2019.Models;

namespace AdventOfCode.Year2019.Day07;

public class AoC201907 : AoCBase
{
    internal static string[] input = Read.InputLines(typeof(AoC201907));
    static ImmutableArray<int> program = input[0].Split(',').Select(int.Parse).ToImmutableArray();

    public override object Part1() => (from p in GetPermutations(Range(0, 5), 5)
                                       select Run(p)).Max();

    static int Run(IEnumerable<int> phaseSettings)
    {
        int next = 0;
        foreach (var i in phaseSettings)
        {
            next = IntCode.Run(program, i, next).Last();
        }
        return next;
    }

    public override object Part2() => -1;
    object Part2WIP() => (from p in GetPermutations(Range(5, 5), 5)
                                       select Run2(p)).Max();
    static int Run2(IEnumerable<int> phaseSettings)
    {
        int next = 0;
        int input = 0;

        var runningAmp = new List<AmplifierState>
            {
                new AmplifierState(5, program.Select(i => (long)i).ToList()),
                new AmplifierState(6, program.Select(i => (long)i).ToList()),
                new AmplifierState(7, program.Select(i => (long)i).ToList()),
                new AmplifierState(8, program.Select(i => (long)i).ToList()),
                new AmplifierState(9, program.Select(i => (long)i).ToList())
            };
        var amplifiers = Range(5, 5).ToDictionary(i => i, _ => new Amplifier(program));
        int iteration = 0;
        while (!runningAmp.All(amp => amp.Finished))
        {
            foreach (var i in phaseSettings)
            {
                iteration++;
                input = Day072019.ModifyArrayMemory(runningAmp[i - 5], i, input).Output;
                next = amplifiers[i].Run(i, next).First();

                if (amplifiers[i].Halted != runningAmp[i - 5].Finished)
                    Debugger.Break();
                if (input != next) 
                    Debugger.Break();
                if (!amplifiers[i].program.SequenceEqual(runningAmp[i - 5].Array.Select(l => (int)l)))
                    Debugger.Break();
            }
        }

        return input;
        

        return next;
    }


    internal static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> list, int length) => length == 1
           ? from t in list select Repeat(t, 1)
           : GetPermutations(list, length - 1).SelectMany(t => list.Where(e => !t.Contains(e)), (t1, t2) => t1.Concat(Repeat(t2, 1)).ToArray());

}

class Amplifier
{
    public int[] program;
    int index = 0;
    bool halted = false;
    public bool Halted => halted;
    Queue<int> inputs = new Queue<int>();
    public Amplifier(IEnumerable<int> program)
    {
        this.program = program.ToArray();
    }
    internal IEnumerable<int> Run(params int[] input)
    {
        int opcode;
        foreach (var i in input)
            inputs.Enqueue(i);
        do
        {
            (opcode, var modes) = Decode(program[index]);
            switch (opcode)
            {
                case 1:
                    {
                        const int parameterCount = 3;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        var result = a + b;
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = result;
                        index += jump;
                    }
                    break;
                case 2:
                    {
                        const int parameterCount = 3;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        var result = a * b;
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = result;
                        index += jump;
                    }
                    break;
                case 3:
                    {
                        if (!inputs.TryDequeue(out int inputvalue)) throw new InvalidOperationException("no more inputs");
                        const int parameterCount = 1;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = inputvalue;
                        index += jump;
                    }
                    break;
                case 4:
                    {
                        const int parameterCount = 1;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        var jump = parameterCount + 1;
                        index += jump;
                        yield return GetValue(program, parameters.First());
                    }
                    break;
                case 5:
                    {
                        const int parameterCount = 2;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        index = a == 0 ? index + parameterCount + 1 : b;
                    }
                    break;
                case 6:
                    {
                        const int parameterCount = 2;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        index = a == 0 ? b : index + parameterCount + 1;
                    }
                    break;
                case 7:
                    {
                        const int parameterCount = 3;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        var result = a < b ? 1 : 0;
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = result;
                        index += jump;
                    }
                    break;
                case 8:
                    {
                        const int parameterCount = 3;
                        var parameters = GetParameters(program, index, modes, parameterCount);
                        (var a, var b) = GetValues(program, parameters);
                        var result = a == b ? 1 : 0;
                        var jump = parameterCount + 1;
                        program[parameters.Last().value] = result;
                        index += jump;
                    }
                    break;
                case 99:
                    halted = true;
                    break;
                default:
                    throw new Exception();
            }
        }
        while (opcode != 99);
        halted = true;
    }

    static IEnumerable<(int value, Mode mode)> GetParameters(IEnumerable<int> program, int index, IEnumerable<Mode> modes, int n)
        => program.Skip(index + 1).Take(n).Zip(modes, (l, r) => (value: l, mode: r));

    static (int opcode, IReadOnlyCollection<Mode> modes) Decode(int value)
    {
        Mode[] modes = new Mode[3];
        var opcode = value % 100;
        value /= 100;
        for (int i = 0; i < 3; i++)
        {
            modes[i] = (Mode)(value % 10);
            value /= 10;
        }
        return (opcode, modes);
    }
    static int GetValue(int[] program, (int index, Mode mode) parameter)
    {
        (var index, var mode) = parameter;
        return mode switch
        {
            Mode.Immediate => index,
            Mode.Position => program[index],
            _ => throw new NotImplementedException()
        };
    }

    static (int a, int b) GetValues(int[] program, IEnumerable<(int, Mode)> parameters)
    {
        var enumerator = parameters.GetEnumerator();
        enumerator.MoveNext();
        var a = GetValue(program, enumerator.Current);
        enumerator.MoveNext();
        var b = GetValue(program, enumerator.Current);
        return (a, b);
    }
}

