using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Year2019;


class IntCode
{
    TextWriter? output;
    public bool IsTerminated { get; private set; }
    private Dictionary<int, long> program;

    Parameter[] parametersBuffer = new Parameter[3];
    Mode[] modes = new Mode[3];
    long[] values = new long[3];

    public IntCode(long[] program, TextWriter? output = null)
    {
        this.program = Range(0, program.Length).ToDictionary(i => i, i => program[i]);
        this.output = output;
    }


    public IEnumerable<long> Run(params long[] input)
    {
        if (input.Length == 0) input = [0L];
        foreach (var i in input)
        {
            while (!IsTerminated)
            {
                var returnValue = Run(0);
                if (!IsTerminated)
                    yield return returnValue;
            }
            IsTerminated = false;
        }
    }

    int index = 0;
    int offset = 0;

    public long Run(long input)
    {
        output?.WriteLine(input);

        long opcode;
        while (!IsTerminated)
        {
            (opcode, var modes) = Decode(program[index]);

            output?.WriteLine((opcode, string.Join(",", modes)));

            if (opcode == 99)
                break;

            var parameters = opcode switch
            {
                1 or 2 or 7 or 8 => GetParameters(index, modes, 3),
                3 or 4 or 9 => GetParameters(index, modes, 1),
                5 or 6 => GetParameters(index, modes, 2)
            };

            output?.WriteLine(string.Join(",", parameters));

            var parameterValues = opcode switch
            {
                1 or 2 or 5 or 6 or 7 or 8 => GetValues(parameters, offset, 2),
                4 or 9 => GetValues(parameters, offset, 1),
                _ => Managed(Array.Empty<long>(), 0)
            };

            int parameterCount = parameters.Count;
            int jump = parameterCount + 1;

            if (opcode == 4)
            {
                index += jump;
                return parameterValues[0];
            }

            long? value = null;
            int? o = null;
            (value, o, jump) = opcode switch
            {
                1 => (parameterValues.Sum(), o, jump),
                2 => (parameterValues.Product(), o, jump),
                3 => (input, o, jump),
                5 when parameterValues[0] != 0 => (value, o, int.CreateChecked(parameterValues[1]) - index),
                6 when parameterValues[0] == 0 => (value, o, int.CreateChecked(parameterValues[1]) - index),
                5 or 6 => (value, o, jump),
                7 => (parameterValues[0] < parameterValues[1] ? 1 : 0, o, jump),
                8 => (parameterValues[0] == parameterValues[1] ? 1 : 0, o, jump),
                9 => (value, int.CreateChecked(parameterValues[0]), jump)
            };

            if (value.HasValue)
            {
                SetValue(parameters[^1], offset, value.Value);
            }
            if (o.HasValue)
            {
                offset += o.Value;
            }

            index += jump;
        }

        IsTerminated = true;
        return long.MinValue;
    }


    (long opcode, Mode[] modes) Decode(long value)
    {
        var opcode = value % 100;
        value /= 100;
        for (long i = 0; i < 3; i++)
        {
            modes[i] = (Mode)(value % 10);
            value /= 10;
        }
        return (opcode, modes);
    }
    ManagedBuffer<Parameter> GetParameters(int index, Mode[] modes, int n)
    {
        for (int i = 0; i < n; i++)
        {
            var m = modes[i];
            var value = program.ContainsKey(index + i + 1) ? program[index + i + 1] : 0;
            parametersBuffer[i] = new(value, m);
        }
        return Managed(parametersBuffer, n);
    }

    void SetValue(Parameter parameter, int offset, long value)
    {
        var (index, _) = parameter.Get(offset);
        program[index] = value;
    }

    long GetValue(Parameter parameter, int offset)
    {
        var (index, value) = parameter.Get(offset);
        return parameter.mode switch
        {
            Mode.Immediate => value,
            _ => program.ContainsKey(index) ? program[index] : 0
        };
    }
    ManagedBuffer<long> GetValues(ManagedBuffer<Parameter> parameters, int offset, int n)
    {
        for (int i = 0; i < n; i++)
        {
            values[i] = GetValue(parameters[i], offset);
        }
        return Managed(values, n);
    }

    static ManagedBuffer<T> Managed<T>(T[] array, int count) => new(array, count);
    struct ManagedBuffer<T>(T[] array, int count) : IEnumerable<T>
    {
        public T this[int index] => index >= 0 && index < count ? array[index] : throw new IndexOutOfRangeException();
        public int Count => count;

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < count; i++)
                yield return this[i];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
    readonly record struct Parameter(long value, Mode mode)
    {
        public (int index, long value) Get(int offset) => mode switch
        {
            Mode.Relative => (int.CreateChecked(value) + offset, -1),
            Mode.Position => (int.CreateChecked(value), -1),
            Mode.Immediate => (-1, value)
        };
        public long Value => value;
    }
    enum Mode
    {
        Position = 0,
        Immediate = 1,
        Relative = 2
    }
}

