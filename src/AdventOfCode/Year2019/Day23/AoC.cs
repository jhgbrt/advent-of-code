namespace AdventOfCode.Year2019.Day23;

public class AoC201923(string[] input)
{
    readonly long[] program = input[0].Split(',').Select(long.Parse).ToArray();
    const int NetworkSize = 50;

    NetworkComputer[] Computers() => [.. Enumerable.Range(0, NetworkSize).Select(i => new NetworkComputer(i, program))];

    public AoC201923() : this(Read.InputLines())
    {
    }

    public long Part1()
    {
        NetworkComputer[] computers = Computers();
        while (true)
        {
            foreach (var computer in computers)
            {
                computer.Step();
                
                while (computer.HasPacket)
                {
                    var packet = computer.GetPacket();
                    
                    if (packet.Address == 255)
                    {
                        return packet.Y;
                    }
                    
                    if (packet.Address >= 0 && packet.Address < NetworkSize)
                    {
                        computers[packet.Address].QueuePacket(packet.X, packet.Y);
                    }
                }
            }
        }
    }
    
    public long Part2()
    {
        NetworkComputer[] computers = Computers();
        Packet? natPacket = null;
        long? lastNatY = null;
        
        while (true)
        {
            bool idle = true;
            
            foreach (var computer in computers)
            {
                computer.Step();
                
                if (!computer.IsIdle)
                {
                    idle = false;
                }
                
                while (computer.HasPacket)
                {
                    var packet = computer.GetPacket();
                    idle = false;
                    
                    if (packet.Address == 255)
                    {
                        natPacket = packet;
                    }
                    else if (packet.Address >= 0 && packet.Address < NetworkSize)
                    {
                        computers[packet.Address].QueuePacket(packet.X, packet.Y);
                    }
                }
            }
            
            if (idle && natPacket.HasValue)
            {
                if (lastNatY == natPacket.Value.Y)
                {
                    return natPacket.Value.Y;
                }
                
                lastNatY = natPacket.Value.Y;
                computers[0].QueuePacket(natPacket.Value.X, natPacket.Value.Y);
            }
        }
    }
}

record struct Packet(int Address, long X, long Y);

class NetworkComputer
{
    readonly IntCodeAsync intcode;
    readonly Queue<long> inputQueue = new();
    readonly Queue<long> outputBuffer = new();
    readonly Queue<Packet> outgoingPackets = new();
    int emptyReadCount = 0;
    
    public bool HasPacket => outgoingPackets.Count > 0;
    public bool IsIdle => emptyReadCount > 0 && inputQueue.Count == 0;
    
    public NetworkComputer(int address, long[] program)
    {
        intcode = new IntCodeAsync(program);
        inputQueue.Enqueue(address);
    }
    
    public void QueuePacket(long x, long y)
    {
        inputQueue.Enqueue(x);
        inputQueue.Enqueue(y);
        emptyReadCount = 0;
    }
    
    public Packet GetPacket() => outgoingPackets.Dequeue();
    
    public void Step()
    {
        var output = intcode.Step(() =>
        {
            if (inputQueue.Count > 0)
            {
                emptyReadCount = 0;
                return inputQueue.Dequeue();
            }
            emptyReadCount++;
            return -1;
        });
        
        if (output.HasValue)
        {
            outputBuffer.Enqueue(output.Value);
            
            if (outputBuffer.Count == 3)
            {
                var address = (int)outputBuffer.Dequeue();
                var x = outputBuffer.Dequeue();
                var y = outputBuffer.Dequeue();
                outgoingPackets.Enqueue(new Packet(address, x, y));
                emptyReadCount = 0;
            }
        }
    }
}

class IntCodeAsync(long[] program)
{
    public bool Halted { get; private set; }
    Dictionary<int, long> program = program.Select((p, i) => (p, i)).ToDictionary(p => p.i, p => p.p);
    int index = 0;
    int offset = 0;

    long this[int position]
    {
        get => program.TryGetValue(position, out var value) ? value : 0;
        set => program[position] = value;
    }
    
    public long? Step(Func<long> getInput)
    {
        if (Halted) return null;
        
        var intvalue = int.CreateChecked(this[index]);
        var opcode = intvalue % 100;
        var a = intvalue / 100 % 10;
        var b = intvalue / 1000 % 10;
        var c = intvalue / 10000 % 10;
        
        var (nofparams, nofargs) = opcode switch
        {
            1 => (3, 2),
            2 => (3, 2),
            3 => (1, 0),
            4 => (1, 1),
            5 => (2, 2),
            6 => (2, 2),
            7 => (3, 2),
            8 => (3, 2),
            9 => (1, 1),
            99 => (0, 0),
            _ => throw new InvalidOperationException($"Unknown opcode {opcode}")
        };
        
        var parameters = new Parameters(
            nofparams < 1 ? default : new(this[index + 1], a),
            nofparams < 2 ? default : new(this[index + 2], b),
            nofparams < 3 ? default : new(this[index + 3], c),
            nofparams);
        
        var args = (
            a: nofargs < 1 ? 0L : GetValue(parameters.a),
            b: nofargs < 2 ? 0L : GetValue(parameters.b)
        );
        
        long? @null = null;
        long? returnValue = null;
        
        (bool halt, long? value, int delta, int jump, returnValue) = opcode switch
        {
            1 => (false, args.a + args.b, 0, nofparams + 1, @null),
            2 => (false, args.a * args.b, 0, nofparams + 1, @null),
            3 => (false, getInput(), 0, nofparams + 1, @null),
            4 => (false, @null, 0, nofparams + 1, args.a),
            5 => (false, @null, 0, args.a != 0 ? int.CreateChecked(args.b) - index : nofparams + 1, @null),
            6 => (false, @null, 0, args.a == 0 ? int.CreateChecked(args.b) - index : nofparams + 1, @null),
            7 => (false, args.a < args.b ? 1L : 0L, 0, nofparams + 1, @null),
            8 => (false, args.a == args.b ? 1L : 0L, 0, nofparams + 1, @null),
            9 => (false, @null, int.CreateChecked(args.a), nofparams + 1, @null),
            99 => (true, @null, 0, 0, @null)
        };
        
        if (halt)
        {
            Halted = true;
            return null;
        }
        
        if (value.HasValue)
        {
            this[GetIndex(parameters.Last)] = value.Value;
        }
        
        offset += delta;
        index += jump;
        
        return returnValue;
        
        long GetValue(Parameter p) => p.mode switch
        {
            1 => p.value,
            0 or 2 => this[GetIndex(p)],
            _ => throw new InvalidOperationException($"Unknown parameter mode {p.mode}")
        };
        
        int GetIndex(Parameter p) => p.mode switch
        {
            0 => int.CreateChecked(p.value),
            2 => int.CreateChecked(p.value) + offset,
            _ => throw new InvalidOperationException($"Unknown parameter mode {p.mode}")
        };
    }
    
    readonly record struct Parameters(Parameter a, Parameter b, Parameter c, int n)
    {
        public Parameter Last => n switch { 1 => a, 2 => b, 3 => c };
    }
    
    readonly record struct Parameter(long value, int mode);
}

public class AoC201923Tests
{
    readonly AoC201923 sut;
    
    public AoC201923Tests()
    {
        var input = Read.InputLines();
        sut = new AoC201923(input);
    }
    
    [Fact]
    public void TestPart1()
    {
        var result = sut.Part1();
        Assert.Equal(23259, result);
    }
    
    [Fact]
    public void TestPart2()
    {
        var result = sut.Part2();
        Assert.Equal(15742, result);
    }
}