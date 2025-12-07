using Net.Code.AdventOfCode.Toolkit.Commands;

using System.Reflection;

namespace Net.Code.AdventOfCode.Toolkit.UnitTests;

public class HtmlToSpectreMarkupTests
{
    private static string LoadEmbeddedHtml(string resourceName)
    {
        using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"Net.Code.AdventOfCode.Toolkit.UnitTests.{resourceName}");
        if (stream == null)
            throw new InvalidOperationException($"Embedded resource {resourceName} not found.");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    [Fact]
    public void Transform_UnlockedPuzzle_ReturnsDescription()
    {
        var html = @"<html><body><main><article class='day-desc'>Part 1 description</article><p>To begin, get your input.</p></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Equal("[dim]Part 1 description[/]", result);
    }

    [Fact]
    public void Transform_UnlockedPuzzle_RealHtml()
    {
        var html = LoadEmbeddedHtml("puzzle-unanswered.html");
        var status = Core.Status.Unlocked;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Contains("[bold]--- Day 9: Sensor Boost ---[/]", result);
    }

    [Fact]
    public void Transform_AnsweredPart1_ReturnsDescriptionAndPart2()
    {
        var html = @"<html><body><main><article class='day-desc'><h2>--- Part 1: Title ---</h2><p>Part 1</p></article><p>Your answer was 123.</p><p class='day-success'>Part 1 complete</p><article class='day-desc'>Part 2</article><form>Answer form</form></main></body></html>";
        var status = Core.Status.AnsweredPart1;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Contains("[dim]Part 1[/]", result);
        Assert.Contains("[dim]Part 2[/]", result);
    }

    [Fact]
    public void Transform_CompletedPuzzle_StopsAtSuccess()
    {
        var html = @"<html><body><main><article class='day-desc'>Part 1</article><p>Answer 1</p><article class='day-desc'>Part 2</article><p>Answer 2</p><p class='day-success'>Complete</p><p>Extra</p></main></body></html>";
        var status = Core.Status.Completed;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.DoesNotContain("[dim]Extra[/]", result);
    }

    [Fact]
    public void Transform_CompletedPuzzle_RealHtml()
    {
        var html = LoadEmbeddedHtml("puzzle-answered-both-parts.html");
        var status = Core.Status.Completed;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Contains("[bold]--- Day 1: Not Quite Lisp ---[/]", result);
        Assert.Contains("[dim]Your puzzle answer was [/]", result);
        Assert.Contains("[dim]232[/]", result);
        Assert.Contains("[bold]--- Part Two ---[/]", result);
        Assert.Contains("[dim]Your puzzle answer was [/]", result);
        Assert.Contains("[dim]1783[/]", result);
        Assert.DoesNotContain("get your puzzle input", result);
    }

    [Fact]
    public void Transform_CodeElement_RendersAsGrey()
    {
        var html = @"<html><body><main><article class='day-desc'><p>Code: <code>example</code></p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Contains("[dim]example[/]", result);
    }

    [Fact]
    public void Transform_Link_AddsHref()
    {
        var html = @"<html><body><main><article class='day-desc'><p><a href='http://example.com'>link</a></p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Contains("[dim]link[/]", result);
        Assert.Contains("[blue]http://example.com[/]", result);
    }

    [Fact]
    public void Transform_NoMain_ReturnsNoContent()
    {
        var html = @"<html><body></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Equal("[dim]No content[/]", result);
    }

    [Fact]
    public void Transform_BoldText_RendersAsBold()
    {
        var html = @"<html><body><main><article class='day-desc'><p><b>some bold text</b></p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Contains("[bold]some bold text[/]", result);
    }

    [Fact]
    public void Transform_CodeWithEmphasis()
    {
        var html = @"<html><body><main><article class='day-desc'><p>Code: <code>example <em>emphasized</em></code></p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Contains("[dim]example [/][bold]emphasized[/]", result);
    }

    [Fact]
    public void Transform_H2Title_RendersAsBold()
    {
        var html = @"<html><body><main><article class='day-desc'><h2>--- Part 1: Some Title ---</h2><p>Description</p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToSpectreMarkup.Transform(html, status);

        Assert.Contains("[bold]--- Part 1: Some Title ---[/]", result);
    }
}

public class HtmlToMarkdownTests
{
    [Fact]
    public void Transform_UnlockedPuzzle_ReturnsDescription()
    {
        var html = @"<html><body><main><article class='day-desc'>Part 1 description</article><p>To begin, get your input.</p></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToMarkdown.Transform(html, status);

        Assert.Equal("Part 1 description\n\n", result);
    }

    [Fact]
    public void Transform_CodeElement_RendersAsCode()
    {
        var html = @"<html><body><main><article class='day-desc'><p>Code: <code>example</code></p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToMarkdown.Transform(html, status);

        Assert.Contains("`example`", result);
    }

    [Fact]
    public void Transform_Link_AddsHref()
    {
        var html = @"<html><body><main><article class='day-desc'><p><a href='http://example.com'>link</a></p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToMarkdown.Transform(html, status);

        Assert.Contains("[link](http://example.com)", result);
    }

    [Fact]
    public void Transform_BoldText_RendersAsBold()
    {
        var html = @"<html><body><main><article class='day-desc'><p><b>some bold text</b></p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToMarkdown.Transform(html, status);

        Assert.Contains("**some bold text**", result);
    }

    [Fact]
    public void Transform_H2Title_RendersAsHeader()
    {
        var html = @"<html><body><main><article class='day-desc'><h2>--- Part 1: Some Title ---</h2><p>Description</p></article></main></body></html>";
        var status = Core.Status.Unlocked;
        var result = HtmlToMarkdown.Transform(html, status);

        Assert.Contains("## --- Part 1: Some Title ---", result);
    }
}