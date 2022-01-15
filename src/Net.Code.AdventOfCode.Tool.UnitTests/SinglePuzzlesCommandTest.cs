using Net.Code.AdventOfCode.Tool.Commands;

using NSubstitute;
using NSubstitute.Extensions;

using Spectre.Console.Cli;

using System;
using System.Threading.Tasks;

using Xunit;

namespace Net.Code.AdventOfCode.Tool.UnitTests;

public class SinglePuzzlesCommandTest
{
    [Fact]
    public async Task NoYearNoDay_Throws()
    {
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings();
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }
    [Fact]
    public async Task YearNoDay_Throws()
    {
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2016 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await Assert.ThrowsAsync<Exception>(() => sut.ExecuteAsync(context, options));
    }

    [Fact]
    public async Task YearDay_RunsSinglePuzzle()
    {
        var sut = Substitute.ForPartsOf<SinglePuzzleCommand<AoCSettings>>();
        var context = new CommandContext(Substitute.For<IRemainingArguments>(), "name", default);
        var options = new AoCSettings { year = 2016, day = 23 };
        await sut.Configure().ExecuteAsync(Arg.Any<int>(), Arg.Any<int>(), options);
        await sut.ExecuteAsync(context, options);
        await sut.Received(1).ExecuteAsync(2016, 23, options);
    }
}
