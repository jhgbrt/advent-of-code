using System.Collections.Generic;
using System.Linq;

public class CPU2 
{
    private readonly int _id;
    private readonly Queue<long> _incoming;
    private readonly Queue<long> _outgoing;

    private readonly Dictionary<char, long> _registers
        = "abcdefghijklmnopqrstuvwxyz".Select(c => (key: c, value: 0L))
            .Concat("0123456789".Select(c => (key: c, value: long.Parse(c.ToString()))))
            .ToDictionary(x => x.key, x => x.value);

    private int _instructionPtr;
    private IReadOnlyList<string> _instructions;

    public int Sent { get; private set; }

    public CPU2(int id, Queue<long> incoming, Queue<long> outgoing)
    {
        _id = id;
        _registers['p'] = id;
        _incoming = incoming;
        _outgoing = outgoing;
    }

    public void Load(IReadOnlyList<string> instructions)
    {
        _instructions = instructions;
        _instructionPtr = 0;
    }
    public void Run()
    {
        do
        {
            var tokens = _instructions[_instructionPtr].Split(' ');
            var (instruction, register) = (tokens[0], tokens[1][0]);
            long value = 0;
            if (tokens.Length > 2 && !long.TryParse(tokens[2], out value))
                value = _registers[tokens[2][0]];

            int offset = 1;
            switch (instruction)
            {
                case "snd":
                    _outgoing.Enqueue(_registers[register]);
                    Sent++;
                    break;
                case "set":
                    _registers[register] = value;
                    break;
                case "add":
                    _registers[register] += value;
                    break;
                case "mul":
                    _registers[register] *= value;
                    break;
                case "mod":
                    _registers[register] %= value;
                    break;
                case "rcv":
                    if (_incoming.Count > 0)
                        _registers[register] = _incoming.Dequeue();
                    else
                        return;
                    break;
                case "jgz":
                    if (_registers[register] > 0)
                        offset = (int) value;
                    break;
            }

            _instructionPtr += offset;
        } while (_instructionPtr >= 0 && _instructionPtr < _instructions.Count);
    }
}