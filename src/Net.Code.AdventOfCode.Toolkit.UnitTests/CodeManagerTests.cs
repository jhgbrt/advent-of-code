using Net.Code.AdventOfCode.Toolkit.Core;

using Net.Code.AdventOfCode.Toolkit.Logic;

using Microsoft.Extensions.Logging;

using NSubstitute;

using Xunit.Abstractions;
using Microsoft.CodeAnalysis;

namespace Net.Code.AdventOfCode.Toolkit.UnitTests;

public class CodeManagerTests(ITestOutputHelper output)
{
    const string code = """
        namespace AoC.Year2021.Day03;

        public class AoC202103
        {
            public AoC202103() : this(Read.InputLines(), Console.Out)
            {
            }
            public AoC202103(string[] input, TextWriter writer)
            {
                this.writer = writer;
                myvariable = input.Select(int.Parse).ToArray();
            }
            TextWriter writer;
            int[] myvariable;
            public object Part1() => Solve(1);
            public object Part2()
            {
                return Solve(2);
            }
            public long Solve(int part)
            {
                return ToLong(part);
            }
            static long ToLong(int i)
            {
               return i;
            }
        }

        record MyRecord();
        class MyClass
        {
        }
        class Tests
        {
            [Fact]
            public void Test1()
            {
                var sut = new AoC202103(new[] { ""1"", ""2"" });
                Assert.Equal(1, sut.Part1());
            }
        }
        """;

    private CodeManager CreateCodeManager(bool codeFolderExists, string code)
    {
        var filesystem = Substitute.For<IFileSystemFactory>();
        var logger = Substitute.For<ILogger<CodeManager>>();
        logger.WhenForAnyArgs(l => l.LogError(message: default))
            .Do(callInfo =>
            {
                var message = callInfo.ArgAt<object>(2);
                output.WriteLine($"{message}");
            });

        var m = new CodeManager(filesystem, logger);

        var codeFolder = Substitute.For<ICodeFolder>();
        var templateFolder = Substitute.For<ITemplateFolder>();
        templateFolder.Notebook.Returns(new FileInfo("aoc.ipynb"));
        templateFolder.Exists.Returns(true);
        templateFolder.Sample.Returns(new FileInfo("sample.txt"));
        codeFolder.Exists.Returns(codeFolderExists);

        filesystem.GetCodeFolder(new(2021, 3)).Returns(codeFolder);
        filesystem.GetTemplateFolder(null).Returns(templateFolder);


        codeFolder.ReadCode().Returns(code);

        return m;
    }

    [Fact]
    public async Task InitializeCode_WhenCodeFolderDoesNotExist_Succeeds()
    {
        CodeManager m = CreateCodeManager(false, code);
        var puzzle = Puzzle.Create(new(2021, 3), "input", Answer.Empty, string.Empty);
        await m.InitializeCodeAsync(puzzle, false, null, s => { });
    }

    [Fact]
    public async Task InitializeCode_WhenCodeFolderExists_WithForce_Succeeds()
    {
        CodeManager m = CreateCodeManager(true, code);

        var puzzle = Puzzle.Create(new(2021, 3), "input", Answer.Empty, string.Empty);
        await m.InitializeCodeAsync(puzzle, true, null, s => { });
    }

    [Fact]
    public async Task InitializeCode_WhenCodeFolderExists_Throws()
    {
        CodeManager m = CreateCodeManager(true, code);
        var puzzle = Puzzle.Create(new (2021, 3), "input", Answer.Empty, string.Empty);
        await Assert.ThrowsAnyAsync<AoCException>(async () => await m.InitializeCodeAsync(puzzle, false, null, s => { }));
    }
    const string HEADER = """
            var (sw, bytes) = (Stopwatch.StartNew(), 0L);
            var filename = args switch
            {
                ["sample"] => "sample.txt",
                _ => "input.txt"
            };
            """;
    const string REPORT0 = """
            Report(0, "", sw, ref bytes);
            """;
            const string REPORT1 = """
            Report(1, part1, sw, ref bytes);
            """;
            const string REPORT2 = """
            Report(2, part2, sw, ref bytes);
            """;
            const string REPORTFUNCTION = """
            void Report<T>(int part, T value, Stopwatch sw, ref long bytes)
            {
                var label = part switch
                {
                    1 => $"Part 1: [{value}]",
                    2 => $"Part 2: [{value}]",
                    _ => "Init"
                };
                var time = sw.Elapsed switch
                {
                    { TotalMicroseconds: < 1 } => $"{sw.Elapsed.TotalNanoseconds:N0} ns",
                    { TotalMilliseconds: < 1 } => $"{sw.Elapsed.TotalMicroseconds:N0} µs",
                    { TotalSeconds: < 1 } => $"{sw.Elapsed.TotalMilliseconds:N0} ms",
                    _ => $"{sw.Elapsed.TotalSeconds:N2} s"
                };
                var newbytes = GC.GetTotalAllocatedBytes(false);
                var memory = (newbytes - bytes) switch
                {
                    < 1024 => $"{newbytes - bytes} B",
                    < 1024 * 1024 => $"{(newbytes - bytes) / 1024:N0} KB",
                    _ => $"{(newbytes - bytes) / (1024 * 1024):N0} MB"
                };
                Console.WriteLine($"{label} ({time} - {memory})");
                bytes = newbytes;
            }
            """;
    [Theory]
    [InlineData(1, """
        public class AoC202103
        {
            public object Part1() => 1;
            public object Part2() => 1;
        }
        """, """
        using System.Diagnostics;

        {{HEADER}}
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 1;
        {{REPORT2}}
        {{REPORTFUNCTION}}
        """)]
    [InlineData(2, """
        namespace AoC.Year2021.Day03;
    
        public class AoC202103
        {
            static string[] input = Read.InputLines();
            public object Part1() => 1;
            public object Part2() => 1;
        }
        """, """
        using System.Diagnostics;

        {{HEADER}}
        string[] input = File.ReadAllLines(filename);
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 1;
        {{REPORT2}}
        {{REPORTFUNCTION}}
        """)]
    [InlineData(3, """
        namespace AoC.Year2021.Day03;
    
        public class AoC202103
        {
            public AoC202103() : this(Read.InputLines()) {}
            const int A = 42;
            Grid grid;
            public AoC202103(string[] input)
            {
                grid = new Grid(input);
            }
            public object Part1() => 1;
            public object Part2() => 1;
        }

        class Grid(string[] input) {}
        """, """
        using System.Diagnostics;

        {{HEADER}}
        var input = File.ReadAllLines(filename);
        var grid = new Grid(input);
        int A = 42;
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 1;
        {{REPORT2}}
        {{REPORTFUNCTION}}

        class Grid(string[] input)
        {
        }
        """)]
    [InlineData(4, """
        namespace AoC.Year2021.Day03;
    
        public class AoC202103(string[] input)
        {
            public AoC202103() : this(Read.InputLines()) {}
            const int A = 42;
            List<int> grid = new List<int>();
            public object Part1() => 1;
            public object Part2() => 1;
        }
        """, """
        using System.Diagnostics;

        {{HEADER}}
        var input = File.ReadAllLines(filename);
        int A = 42;
        List<int> grid = new List<int>();
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 1;
        {{REPORT2}}
        {{REPORTFUNCTION}}
        """)]
    [InlineData(5, """
        namespace AoC.Year2021.Day03;
    
        public class AoC202103(string[] input, TextWriter writer)
        {
            public AoC202103() : this(Read.InputLines(), Console.Out) {}
            List<int> grid = new List<int>();
            TextWriter writer;
            public object Part1() => 1;
            public object Part2() => 1;
        }
        """, """
        using System.Diagnostics;

        {{HEADER}}
        var input = File.ReadAllLines(filename);
        var writer = Console.Out;
        List<int> grid = new List<int>();
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 1;
        {{REPORT2}}
        {{REPORTFUNCTION}}
        """)]
    [InlineData(6, """
        namespace AoC.Year2021.Day03;
    
        public class AoC202103
        {
            public object Part1() => Solve(1);
            public object Part2() => Solve(2);
            private object Solve(int i) { return i; }
        }
        """, """
        using System.Diagnostics;

        {{HEADER}}
        {{REPORT0}}
        var part1 = Solve(1);
        {{REPORT1}}
        var part2 = Solve(2);
        {{REPORT2}}
        object Solve(int i)
        {
            return i;
        }

        {{REPORTFUNCTION}}
        """)]
    [InlineData(7, """
        public class AoC202103
        {
            string[] input;
            public AoC202103() 
            {
                input = Read.InputLines();
            }
            public object Part1() => 1;
            public object Part2() => 2;
        }
        """, """
        using System.Diagnostics;

        {{HEADER}}
        var input = File.ReadAllLines(filename);
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 2;
        {{REPORT2}}
        {{REPORTFUNCTION}}
        """)]
    [InlineData(8, """
        public class AoC202103
        {
            public AoC202318():this(Read.InputLines(), Console.Out) {}
            readonly TextWriter writer;
            string[] input;
            readonly ImmutableArray<string> items;
            public AoC202318(string[] input, TextWriter writer)
            {
                this.input = input;
                items = input.Select(s =>
                    {
                        return s;
                    }
                ) .ToImmutableArray();
                this.writer = writer;
            }
            public object Part1() => 1;
            public object Part2() => 2;
        }
        """, """
        using System.Diagnostics;
        using System.Collections.Immutable;

        {{HEADER}}
        var input = File.ReadAllLines(filename);
        var writer = Console.Out;
        var items = input.Select(s =>
        {
            return s;
        }).ToImmutableArray();
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 2;
        {{REPORT2}}
        {{REPORTFUNCTION}}
        """)]
    [InlineData(9, """
        public class AoC202103(string[] input, TextWriter writer)
        {
            public AoC202318():this(Read.InputLines(), Console.Out) {}
            readonly TextWriter writer;
            string[] input;
            readonly ImmutableArray<string> items = input.Select(s =>
            {
                return s;
            }) .ToImmutableArray();;
            public object Part1() => 1;
            public object Part2() => 2;
        }
        """, """
        using System.Diagnostics;
        using System.Collections.Immutable;

        {{HEADER}}
        var input = File.ReadAllLines(filename);
        var writer = Console.Out;
        ImmutableArray<string> items = input.Select(s =>
        {
            return s;
        }).ToImmutableArray();
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 2;
        {{REPORT2}}
        {{REPORTFUNCTION}}
        """)]
    [InlineData(10, """
        public class AoC202103
        {
            public AoC202103() : this(42) {}
            public AoC202103(int value)
            {
                this.parameter = value;
            }
            readonly int parameter;
            public object Part1() => 1;
            public object Part2() => 1;
        }
        """, """
        using System.Diagnostics;

        {{HEADER}}
        var value = 42;
        var parameter = value;
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 1;
        {{REPORT2}}
        {{REPORTFUNCTION}}
        """)]
    [InlineData(11, """
        public class AoC202103
        {
            public AoC202103() : this(ComputeValue()) {}
            static int ComputeValue() => Max(0, 42);
            Range r = [0..10];
            public AoC202103(int value)
            {
                this.parameter = value;
            }
            readonly int parameter;
            public object Part1() => 1;
            public object Part2() => 1;
        }
        """, """
        using System.Diagnostics;
        using static System.Math;

        var (sw, bytes) = (Stopwatch.StartNew(), 0L);
        var filename = args switch
        {
            ["sample"] => "sample.txt",
            _ => "input.txt"
        };
        var value = ComputeValue();
        var parameter = value;
        Range r = [0..10];
        {{REPORT0}}
        var part1 = 1;
        {{REPORT1}}
        var part2 = 1;
        {{REPORT2}}
        int ComputeValue() => Max(0, 42);
        {{REPORTFUNCTION}}
        """)]
    [InlineData(12, """
        public class AoC202103
        {
            public AoC202103() : this(ComputeValue()) {}
            static int ComputeValue() => 42;
            public AoC202103(int value)
            {
            }
            public object Part1() => 0;
            public object Part2() => 0;
        }
        """, """
        using System.Diagnostics;

        {{HEADER}}
        var value = ComputeValue();
        {{REPORT0}}
        var part1 = 0;
        {{REPORT1}}
        var part2 = 0;
        {{REPORT2}}
        int ComputeValue() => 42;
        {{REPORTFUNCTION}}
        """)]
    public async Task GenerateCodeTests(int n, string input, string expectedTemplate)
    {
        var m = CreateCodeManager(true, input);

        var code = await m.GenerateCodeAsync(new(2021, 3));
        output.WriteLine($"--- Result - test case {n} ---");
        output.WriteLine(code);
        output.WriteLine($"--- Expected - test case {n} ---");

        var expected = expectedTemplate
            .Replace("{{HEADER}}", HEADER)
            .Replace("{{REPORT0}}", REPORT0)
            .Replace("{{REPORT1}}", REPORT1)
            .Replace("{{REPORT2}}", REPORT2)
            .Replace("{{REPORTFUNCTION}}", REPORTFUNCTION);

        output.WriteLine(expected);

        Assert.Equal(expected, code);

        //GC.KeepAlive(n); // only there to identify the test
    }

    [Fact]
    public async Task GenerateCodeTest()
    {
        var m = CreateCodeManager(true, code);

        var result = await m.GenerateCodeAsync(new(2021, 3));
        output.WriteLine(result);
        Assert.Equal("""
            using System.Diagnostics;

            var (sw, bytes) = (Stopwatch.StartNew(), 0L);
            var filename = args switch
            {
                ["sample"] => "sample.txt",
                _ => "input.txt"
            };
            var input = File.ReadAllLines(filename);
            var writer = Console.Out;
            var myvariable = input.Select(int.Parse).ToArray();
            Report(0, "", sw, ref bytes);
            var part1 = Solve(1);
            Report(1, part1, sw, ref bytes);
            var part2 = Part2();
            Report(2, part2, sw, ref bytes);
            object Part2()
            {
                return Solve(2);
            }

            long Solve(int part)
            {
                return ToLong(part);
            }
            
            long ToLong(int i)
            {
                return i;
            }

            void Report<T>(int part, T value, Stopwatch sw, ref long bytes)
            {
                var label = part switch
                {
                    1 => $"Part 1: [{value}]",
                    2 => $"Part 2: [{value}]",
                    _ => "Init"
                };
                var time = sw.Elapsed switch
                {
                    { TotalMicroseconds: < 1 } => $"{sw.Elapsed.TotalNanoseconds:N0} ns",
                    { TotalMilliseconds: < 1 } => $"{sw.Elapsed.TotalMicroseconds:N0} µs",
                    { TotalSeconds: < 1 } => $"{sw.Elapsed.TotalMilliseconds:N0} ms",
                    _ => $"{sw.Elapsed.TotalSeconds:N2} s"
                };
                var newbytes = GC.GetTotalAllocatedBytes(false);
                var memory = (newbytes - bytes) switch
                {
                    < 1024 => $"{newbytes - bytes} B",
                    < 1024 * 1024 => $"{(newbytes - bytes) / 1024:N0} KB",
                    _ => $"{(newbytes - bytes) / (1024 * 1024):N0} MB"
                };
                Console.WriteLine($"{label} ({time} - {memory})");
                bytes = newbytes;
            }
            
            record MyRecord();
            class MyClass
            {
            }
            """, result, ignoreLineEndingDifferences: true);

    }
    [Fact]
    public void GenerateCodeWithMissingUsing()
    {
        var input = """
            namespace AoC.Year2021.Day03;
            public class AoC202103
            {
                public AoC202103() : this(Read.InputLines(), Console.Out)
                {
                }
                public AoC202103(string[] input, TextWriter writer)
                {
                    this.writer = writer;
                    myvariable = input.Select(int.Parse).ToArray();
                }
                TextWriter writer;
                int[] myvariable;
                public object Part1() => Solve(1);
                public object Part2()
                {
                    var max = Max(1 ,2);
                    var range = Range(1, 10);
                    return Solve(2);
                }
                public long Solve(int part)
                {
                    return ToLong(part);
                }
                static long ToLong(int i) => i;
            }
            """;
        var m = CreateCodeManager(true, input);


    }

}


