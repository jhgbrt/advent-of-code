namespace AdventOfCode.Year2019.Day21;

public class AoC201921(string[] input)
{
    public AoC201921() : this(Read.InputLines()) { }

    private readonly long[] program = input.Single().Split(',').Select(long.Parse).ToArray();

    public long Part1() => new IntCode(program).Run(SpringScript.Walk.ToAsciiInput()).Last(v => v > 127);

    public long Part2() => new IntCode(program).Run(SpringScript.Run.ToAsciiInput()).Last(v => v > 127);
}

public class AoC201921Tests
{
    private readonly SpringScriptInterpreter interpreter = new();

    [Fact]
    public void Part1()
    {
        var aoc = new AoC201921();
        Assert.Equal(19352720L, aoc.Part1());
    }

    [Fact]
    public void Part2()
    {
        var aoc = new AoC201921();
        Assert.Equal(1143652885L, aoc.Part2());
    }

    [Fact]
    public void TestAndInstruction()
    {
        // Tiny sanity check for the SpringScript interpreter itself.
        var script = new SpringScript("""
            NOT A J
            AND D J
            WALK
        """.Replace("\r\n", "\n"));

        SpringScriptInterpreter.Sensors sensors = interpreter.AllGround() with { A = false, D = true };
        Assert.True(interpreter.Evaluate(script, sensors));
    }

    [Fact]
    public void WalkScript_MatchesExpectedBooleanLogic()
    {
        foreach (var sensors in interpreter.AllSensorCombinations())
        {
            var expected = (!sensors.A || !sensors.B || !sensors.C) && sensors.D;
            var actual = interpreter.Evaluate(SpringScript.Walk, sensors);
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void RunScript_MatchesExpectedBooleanLogic()
    {
        foreach (var sensors in interpreter.AllSensorCombinations())
        {
            var expected = (!sensors.A || !sensors.B || !sensors.C) && sensors.D && (sensors.E || sensors.H);
            var actual = interpreter.Evaluate(SpringScript.Run, sensors);
            Assert.Equal(expected, actual);
        }
    }

    [Fact]
    public void WalkScript_RespectsInstructionLimits()
    {
        AssertWithinLimits(SpringScript.Walk);
    }

    [Fact]
    public void RunScript_RespectsInstructionLimits()
    {
        AssertWithinLimits(SpringScript.Run);
    }

    [Fact]
    public void WalkScript_RunsOnPuzzleInput_AndProducesDamageOutput()
    {
        var outputs = RunSpringdroid(SpringScript.Walk);
        Assert.Contains(outputs, o => o > 127);
    }

    [Fact]
    public void RunScript_RunsOnPuzzleInput_AndProducesDamageOutput()
    {
        var outputs = RunSpringdroid(SpringScript.Run);
        Assert.Contains(outputs, o => o > 127);
    }

    private static List<long> RunSpringdroid(SpringScript springScript)
    {
        var intcode = new IntCode(Read.InputText().Trim().Split(',').Select(long.Parse));
        return intcode.Run(springScript.ToAsciiInput()).ToList();
    }

    private static void AssertWithinLimits(SpringScript springScript)
    {
        var lines = springScript.Lines;

        // max 15 instructions (excluding WALK/RUN) and max 20 chars/line.
        Assert.True(lines.Length > 0);

        var last = springScript[lines[^1]];
        Assert.True(SpringScriptInterpreter.IsWalkOrRun(last));

        foreach (var line in lines)
        {
            Assert.InRange(springScript[line].Length, 1, 20);
        }

        var instructionCount = lines.Length - 1;
        Assert.InRange(instructionCount, 1, 15);
    }
}

public class SpringScriptInterpreter
{
    public bool Evaluate(SpringScript springScript, Sensors sensors)
    {
        var lines = springScript.Lines;
        bool t = false;
        bool j = false;

        foreach (var range in lines)
        {
            var line = springScript[range];

            if (IsWalkOrRun(line))
            {
                break;
            }

            var remaining = line;
            var op = ReadToken(ref remaining);
            var x = ReadToken(ref remaining);
            var y = ReadToken(ref remaining);

            if (!remaining.Trim().IsEmpty)
            {
                throw new FormatException($"Invalid instruction: '{line.ToString()}'");
            }

            if (y.Length != 1 || (y[0] is not ('T' or 'J')))
            {
                throw new FormatException($"Invalid destination register: '{y.ToString()}'");
            }

            bool GetValue(ReadOnlySpan<char> token)
            {
                if (token.Length != 1)
                {
                    throw new FormatException($"Invalid source register: '{token.ToString()}'");
                }

                return token[0] switch
                {
                    'A' => sensors.A,
                    'B' => sensors.B,
                    'C' => sensors.C,
                    'D' => sensors.D,
                    'E' => sensors.E,
                    'F' => sensors.F,
                    'G' => sensors.G,
                    'H' => sensors.H,
                    'I' => sensors.I,
                    'T' => t,
                    'J' => j,
                    _ => throw new FormatException($"Invalid source register: '{token.ToString()}'")
                };
            }

            bool GetDest(char dest) => dest == 'T' ? t : j;
            void SetDest(char dest, bool value)
            {
                if (dest == 'T') t = value;
                else j = value;
            }

            var xv = GetValue(x);
            var yv = GetDest(y[0]);

            SetDest(y[0], op switch
            {
                var s when s.SequenceEqual("AND") => xv && yv,
                var s when s.SequenceEqual("OR") => xv || yv,
                var s when s.SequenceEqual("NOT") => !xv,
                _ => throw new FormatException($"Invalid opcode: '{op.ToString()}'")
            });
        }

        return j;
    }

    public static bool IsWalkOrRun(ReadOnlySpan<char> line)
        => line.SequenceEqual("WALK") || line.SequenceEqual("RUN");

    private static ReadOnlySpan<char> ReadToken(ref ReadOnlySpan<char> s)
    {
        s = s.TrimStart();
        if (s.IsEmpty)
        {
            throw new FormatException("Unexpected end of instruction");
        }

        var nextSpace = s.IndexOf(' ');
        if (nextSpace < 0)
        {
            var token = s;
            s = ReadOnlySpan<char>.Empty;
            return token;
        }

        var token2 = s[..nextSpace];
        s = s[(nextSpace + 1)..];
        return token2;
    }

    public IEnumerable<Sensors> AllSensorCombinations()
    {
        for (var mask = 0; mask < (1 << 9); mask++)
        {
            yield return new Sensors(
                A: (mask & (1 << 0)) != 0,
                B: (mask & (1 << 1)) != 0,
                C: (mask & (1 << 2)) != 0,
                D: (mask & (1 << 3)) != 0,
                E: (mask & (1 << 4)) != 0,
                F: (mask & (1 << 5)) != 0,
                G: (mask & (1 << 6)) != 0,
                H: (mask & (1 << 7)) != 0,
                I: (mask & (1 << 8)) != 0
            );
        }
    }

    public Sensors AllGround()
        => new(A: true, B: true, C: true, D: true, E: true, F: true, G: true, H: true, I: true);

    public readonly record struct Sensors(
        bool A,
        bool B,
        bool C,
        bool D,
        bool E,
        bool F,
        bool G,
        bool H,
        bool I);
}

public class SpringScript(string program)
{
    private readonly ScriptData data = ScriptData.Create(program);

    public ReadOnlySpan<Range> Lines => data.LineRanges;
    public ReadOnlySpan<char> this[Range range] => program[range];

    public static SpringScript Walk => new("""
        NOT A J
        NOT B T
        OR T J
        NOT C T
        OR T J
        AND D J
        WALK
    """.Replace("\r\n", "\n"));

    public static SpringScript Run => new("""
        NOT A J
        NOT B T
        OR T J
        NOT C T
        OR T J
        AND D J
        NOT E T
        NOT T T
        OR H T
        AND T J
        RUN
    """.Replace("\r\n", "\n"));

    public long[] ToAsciiInput() => data.Program.Select(c => (long)c).ToArray();

    private static string Normalize(string program)
    {
        program = program.Replace("\r\n", "\n");
        return program.EndsWith("\n") ? program : program + "\n";
    }

    private readonly record struct ScriptData(string Program, Range[] LineRanges)
    {
        public static ScriptData Create(string program)
        {
            var normalized = Normalize(program);
            return new(normalized, GetLineRanges(normalized));
        }
    }

    private static Range[] GetLineRanges(string program)
    {
        var count = program.Count('\n');
        List<Range> ranges = new(count + 1);
        var i = 0;

        while (i < program.Length)
        {
            var lineStart = i;
            var lineEnd = program.IndexOf('\n', i);
            if (lineEnd < 0) lineEnd = program.Length;

            var start = lineStart;
            var end = lineEnd;

            while (start < end && char.IsWhiteSpace(program[start])) start++;
            while (end > start && char.IsWhiteSpace(program[end - 1])) end--;

            if (end > start)
            {
                ranges.Add(start..end);
            }

            i = lineEnd + 1;
        }

        return ranges.ToArray();
    }
}



