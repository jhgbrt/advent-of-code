using Newtonsoft.Json.Linq;

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

    Parameter[] parameters = new Parameter[3];
    Mode[] modes = new Mode[3];
    long[] values = new long[3];

    public IntCode(long[] program, TextWriter? output = null)
    {
        this.program = Range(0, program.Length).ToDictionary(i => i, i => program[i]);
        this.output = output;
    }

    public IEnumerable<long> Run(params long[] input)
    {
        if (input.Length == 0) input = new[] { 0L };
        foreach (var i in input)
        {
            while (!IsTerminated)
            {
                var returnValue = Run(i);
                if (returnValue.HasValue)
                    yield return returnValue!.Value;
            }
            IsTerminated = false;
        }
    }

    int index = 0;
    int offset = 0;

    public long? Run(long input)
    {
        output?.WriteLine(input);
        long? returnValue = null;
        while (!returnValue.HasValue)
        {
            long opcode;
            opcode = Decode(program[index]);

            output?.WriteLine((opcode, string.Join(",", modes)));

            if (opcode == 99)
            {
                IsTerminated = true;
                break;
            }

            var (np, nv) = opcode switch
            {
                1 => (3, 2),
                2 => (3, 2),
                3 => (1, 0),
                4 => (1, 1),
                5 => (2, 2),
                6 => (2, 2),
                7 => (3, 2),
                8 => (3, 2),
                9 => (1, 1)
            };

            var parameters = GetParameters(index, np);

            var values = GetValues(parameters, offset, nv);

            output?.WriteLine(string.Join(",", parameters));
            output?.WriteLine(string.Join(",", values));


            int jump = parameters.Count + 1;
            long? value = null;
            int? o = null;

            (value, o, jump, returnValue) = opcode switch
            {
                1 => (values.Sum(), o, jump, returnValue),
                2 => (values.Product(), o, jump, returnValue),
                3 => (input, o, jump, returnValue),
                4 => (value, o, jump, values[0]),
                5 when values[0] != 0 => (value, o, int.CreateChecked(values[1]) - index, returnValue),
                6 when values[0] == 0 => (value, o, int.CreateChecked(values[1]) - index, returnValue),
                5 or 6 => (value, o, jump, returnValue),
                7 => (values[0] < values[1] ? 1 : 0, o, jump, returnValue),
                8 => (values[0] == values[1] ? 1 : 0, o, jump, returnValue),
                9 => (value, int.CreateChecked(values[0]), jump, returnValue)
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
        return returnValue;
    }


    internal long Decode(long value)
    {
        var opcode = value % 100;
        value /= 100;
        for (long i = 0; i < 3; i++)
        {
            modes[i] = (Mode)(value % 10);
            value /= 10;
        }
        return opcode;
    }

    ManagedBuffer<Parameter> GetParameters(int index, int n)
    {
        for (int i = 0; i < n; i++)
        {
            var m = modes[i];
            var value = program.ContainsKey(index + i + 1) ? program[index + i + 1] : 0;
            parameters[i] = new(value, m);
        }
        return Managed(parameters, n);
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
            Mode.Position => (int.CreateChecked(value), -1),
            Mode.Immediate => (-1, value),
            Mode.Relative => (int.CreateChecked(value) + offset, -1)
        };
        public long Value => value;
    }
    internal enum Mode
    {
        Position = 0,
        Immediate = 1,
        Relative = 2
    }
}

