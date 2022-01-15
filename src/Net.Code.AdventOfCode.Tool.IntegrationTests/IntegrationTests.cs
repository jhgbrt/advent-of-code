using Net.Code.AdventOfCode.Tool.Commands;
using Net.Code.AdventOfCode.Tool.Core;
using Net.Code.AdventOfCode.Tool.Logic;

using NSubstitute;

using Spectre.Console;
using Spectre.Console.Rendering;

using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Xunit;
using Xunit.Abstractions;

namespace Net.Code.AdventOfCode.Tool.UnitTests;

public class IntegrationTests
{
    ITestOutputHelper output;

    public IntegrationTests(ITestOutputHelper output)
    {
        this.output = output;
    }

    class TestOutputService : IInputOutputService
    {
        ITestOutputHelper output;

        public TestOutputService(ITestOutputHelper output)
        {
            this.output = output;
        }

        public void MarkupLine(string markup)
        {
            output.WriteLine(markup);
        }

        public T Prompt<T>(IPrompt<T> prompt)
        {
            return default!;
        }

        public void Write(IRenderable renderable)
        {
            output.WriteLine(renderable.ToString());
        }

        public void WriteLine(string message)
        {
            output.WriteLine(message);
        }
    }

    [Fact]
    public async Task Test()
    {
        var resolver = Substitute.For<IAssemblyResolver>();
        var assembly = Assembly.GetExecutingAssembly();
        resolver.GetEntryAssembly().Returns(assembly);
        var io = new TestOutputService(output);
        output.WriteLine(Environment.CurrentDirectory);

        if (Directory.Exists(".cache"))
        {
            Directory.Delete(".cache", true);
        }
        if (Directory.Exists("Year2017"))
        {
            Directory.Delete("Year2017", true);
        }

        var result = await AoC.RunAsync(resolver, io, new[] { "--help" });
        Assert.Equal(0, result);
        result = await AoC.RunAsync(resolver, io, new[] { "init", "2017", "2" });
        Assert.Equal(0, result);
        result = await AoC.RunAsync(resolver, io, new[] { "init", "2017", "1", "--force"});
        Assert.Equal(0, result);
        result = await AoC.RunAsync(resolver, io, new[] { "sync", "2017", "1" });
        Assert.Equal(0, result);
        result = await AoC.RunAsync(resolver, io, new[] { "run", "2017", "1" });
        Assert.Equal(0, result);
        result = await AoC.RunAsync(resolver, io, new[] { "stats" });
        Assert.Equal(0, result);
        await AoC.RunAsync(resolver, io, new[] { "post", "123" });
        Assert.Equal(0, result);
        await AoC.RunAsync(resolver, io, new[] { "verify", "2017", "1" });
    }
}

