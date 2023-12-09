using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace AdventOfCode.Year2016.Day23
{
    using static SyntaxFactory;
    public class AoC201623
    {
        static string[] input = Read.InputLines();

        static ImmutableArray<Instruction> instructions
            = (from line in input
              let split = line.Split(' ')
              let instruction = split[0]
              let first = split[1]
              let second = split.Length > 2 ? split[2] : string.Empty
              select new Instruction(instruction, first, second)).ToImmutableArray();
        
        public object Part1() => new Computer(instructions, 1).Compute(7, 0, 0, 0); // 80*77 + Factorial(7);

        public object Part2() => Factorial(12) + int.Parse(instructions[19].first) * int.Parse(instructions[20].first); // new Computer(instructions, 1).Compute(12, 0, 0, 0); 

        private long Factorial(int num)
        {
            int result = 1;
            for (int i = num; i > 0; i--)
                result *= i;
            return result;
        }
        
       

    }


    class Computer
    {
        int version;
        ImmutableArray<Instruction> instructions;
        public Computer(ImmutableArray<Instruction> instructions, int version)
        {
            this.version = version;
            this.instructions = instructions;
        }
        public int Version => version;

        public (int a, int b, int c, int d) Memory => (memory['a'], memory['b'], memory['c'], memory['d']);


        Dictionary<char, int> memory = new()
        {
            ['a'] = 0,
            ['b'] = 0,
            ['c'] = 0,
            ['d'] = 0
        };
        int GetValue(string input) => input[0] switch
        {
            'a' or 'b' or 'c' or 'd' => memory[input[0]],
            _ => int.Parse(input)
        };
        void SetValue(char register, int value)
        {
            if (memory.ContainsKey(register))
                memory[register] = value;
        }

        public Func<int, int, int, int, (int a, int b, int c, int d, bool shouldcontinue)> GenerateDelegate(int i)
        {
            var code = ToCCode(i);
            var tree = ParseSyntaxTree(code);

            var mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { tree }, references: new[] { mscorlib }, options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var ms = new MemoryStream();
            var emitResult = compilation.Emit(ms);

            if (!emitResult.Success)
            {
                foreach (var d in emitResult.Diagnostics)
                {
                    Console.WriteLine(d);
                }
            }
            var assembly = Assembly.Load(ms.ToArray());
            var type = assembly.GetType("AdventOfCode.Year2016.Day23.Generated.Engine")!;
            var method = type.GetMethod($"Compute{version}", BindingFlags.Static | BindingFlags.Public);
            return method!.CreateDelegate<Func<int, int, int, int, (int a, int b, int c, int d, bool shouldcontinue)>>();
        }

        public int Compute(int a, int b, int c, int d)
        {
            bool log = false;
            int logged = 0;

            memory['a'] = a;
            memory['b'] = b;
            memory['c'] = c;
            memory['d'] = d;
            int steps = 0;
            var i = 0;

            var before = Memory;
            while (i < instructions.Length)
            {
                (var instruction, var first, var second) = instructions[i];
                switch (instruction)
                {
                    case "cpy" when first.Length == 1 && char.IsLetter(first[0]):
                        SetValue(second[0], GetValue(first));
                        i++;
                        break;
                    case "cpy":
                        SetValue(second[0], GetValue(first));
                        i++;
                        break;
                    case "inc":
                        SetValue(first[0], GetValue(first) + 1);
                        i++;
                        break;
                    case "dec":
                        SetValue(first[0], GetValue(first) - 1);
                        i++;
                        break;
                    case "jnz" when GetValue(first) != 0:
                        i += GetValue(second);
                        break;
                    case "tgl":
                        Console.WriteLine($"[{i}] VERSION: {version} -> {version + 1} - {Memory}");
                        instructions = Toggle(instructions, i, GetValue(first));
                        version++;
                        i++;
                        //log = true;
                        break;
                    default:
                        i++;
                        break;

                }
                steps++;
                if (log && logged < 20)
                {
                    Console.WriteLine($"[{i}] - {instructions[i]} - {Memory}");
                    logged++;
                }
            }
            Console.WriteLine($"{before} -> {Memory}");
            return memory['a'];

        }
        private static ImmutableArray<Instruction> Toggle(ImmutableArray<Instruction> instructions, int i, int value)
        {
            var j = i + value;

            if (j < 0 || j >= instructions.Length)
            {
                return instructions;
            }

            var current = instructions[j];

            Instruction toggled = current switch
            {
                { instruction: "inc", second: "" } => new("dec", current.first, ""),
                { second: "" } => new("inc", current.first, ""),
                { instruction: "jnz", second: not "" } => new("cpy", current.first, current.second),
                { second: not "" } => new("jnz", current.first, current.second)
            };
            instructions = instructions.SetItem(j, toggled);

            return instructions;
        }

        public string ToCCode(int startAt)
        {
            var sb = new StringBuilder();
            sb.AppendLine($$"""
            namespace AdventOfCode.Year2016.Day23.Generated
            {
                public partial class Engine
                {
                    public static (int a, int b, int c, int d, bool shouldcontinue) Compute{{version}}(int a, int b, int c, int d)
                    {
                        int i = -1;
                        bool shouldcontinue = false;
            """
            );
            if (startAt>0)
                sb.AppendLine($"goto __{startAt};");
            for (int i = 0; i < instructions.Length; i++)
            {
                sb.Append($"__{i}: ");
                sb.Append($"i = {i}; ");
                //sb.Append($"Log({i}, a, b, c, d, \"{instructions[i].ToString()}\"); ");
                sb.Append(instructions[i].ToCCode(i));
                sb.AppendLine();
            }
            sb.AppendLine("goto _return;");
            foreach (var c in new[] { 'a', 'b', 'c', 'd' })
            {
                sb.Append($"__jnz_{c}: ");
                for (int i = 0; i < instructions.Length; i++)
                {
                    sb.Append($"if ({c} + i == {i}) {{ Console.WriteLine($\"=== GOTO {i} === ({{a}} {{b}} {{c}} {{d}}) \"); goto __{i}; }}");
                }
                sb.AppendLine();
            }
            sb.AppendLine("_return: return (a,b,c,d, shouldcontinue);");

            sb.AppendLine($$"""
                    }
                }
            }
            """);


            return sb.ToString();
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach (var i in instructions)
                sb.AppendLine(i.ToString());
            return sb.ToString();
        }
    }

    internal record struct Instruction(string instruction, string first, string second)
    {
        public override string ToString() => $"{instruction} {first} {second}";
        public Instruction Toggle => (instruction, second) switch
        {
            { instruction: "inc", second: "" } => new("dec", first, ""),
            { second: "" } => new("inc", first, ""),
            { instruction: "jnz", second: not "" } => new("cpy", first, second),
            { second: not "" } => new("jnz", first, second)
        };

        public string ToCCode(int i)
        {
            return instruction switch
            {
                "inc" => $"{first}++;",
                "dec" => $"{first}--;",
                "cpy" => $"{second} = {first};",
                "jnz" when int.TryParse(second, out var v) => $"if ({first} != 0) goto __{i + (v < 0 ? v : v + 1)};",
                "jnz" => $"if ({first} != 0) goto __jnz_{second};",
                "tgl" => $"i = {i}; shouldcontinue = true; goto _return;",
                _ => this.ToString()
            };
        }

    }
}

