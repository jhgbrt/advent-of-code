namespace Net.Code.AdventOfCode.Toolkit.Commands;

using Net.Code.AdventOfCode.Toolkit.Core;
using Net.Code.AdventOfCode.Toolkit.Infrastructure;
using System.Text;

using Spectre.Console.Rendering;
using Spectre.Console;
using Spectre.Console.Cli;

using System;
using System.ComponentModel;
using System.Threading;
using HtmlAgilityPack;

[Description("Render the instructions.")]
class Show(IPuzzleManager puzzleManager, AoCLogic aocLogic, IInputOutputService io) : ManyPuzzlesCommand<Show.Settings>(aocLogic)
{
    public class Settings : AoCSettings
    {
        [Description("Output format: spectre (default) or md")]
        [CommandOption("--format")]
        public string Format { get; set; } = "spectre";
    }

    public override async Task<int> ExecuteAsync(PuzzleKey key, Settings options, CancellationToken ct)
    {
        var puzzle = await puzzleManager.SyncPuzzle(key);

        var markupString = options.Format switch
        {
            "md" => HtmlToMarkdown.Transform(puzzle.Html, puzzle.Status),
            _ => HtmlToSpectreMarkup.Transform(puzzle.Html, puzzle.Status)
        };

        io.Write(new Markup(markupString));
        return 0;
    }
}


internal static class HtmlToSpectreMarkup
{
    public static string Transform(string html, Core.Status status)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        var doc = AdventOfCodeDocument.LoadFrom(htmlDoc, status);
        return RenderDocument(doc);
    }

    private static string RenderDocument(AdventOfCodeDocument doc)
    {
        var sb = new StringBuilder();
        foreach (var child in doc.Children)
        {
            sb.Append(RenderElement(child));
        }
        if (sb.Length == 0)
            sb.Append("[dim]No content[/]");
        return sb.ToString();
    }

    private static string RenderElement(DocumentElement elem)
    {
        switch (elem)
        {
            case Heading h:
                return $"\n[bold]{EscapeMarkup(h.Text)}[/]\n\n";
            case Paragraph p:
                var markup = string.Concat(p.Inlines.Select(RenderInline));
                return markup + "\n";
            case List l:
                return RenderList(l, 0);
            case Article a:
                return string.Concat(a.Children.Select(RenderElement));
            case InlineContent c:
                return string.Concat(c.Inlines.Select(RenderInline));
            default:
                return string.Empty;
        }
    }

    private static string RenderInline(InlineElement elem)
    {
        switch (elem)
        {
            case Text t:
                return $"[dim]{EscapeMarkup(t.Content)}[/]";
            case Bold b:
                return $"[bold]{EscapeMarkup(b.Content)}[/]";
            case Italic i:
                var style = i.IsStarStyled ? "yellow" : "bold";
                return $"[{style}]{EscapeMarkup(i.Content)}[/]";
            case Code c:
                var codeInner = string.Concat(c.Inlines.Select(RenderInline));
                return codeInner;
            case Link l:
                var linkInner = string.Concat(l.Inlines.Select(RenderInline));
                if (!string.IsNullOrEmpty(l.Href))
                {
                    linkInner += $"[dim] ([/][blue]{EscapeMarkup(l.Href)}[/][dim])[/]";
                }
                return linkInner;
            case EasterEgg e:
                return $"[dim]{EscapeMarkup(e.VisibleText)}[/][dim] ({EscapeMarkup(e.Tooltip)})[/]";
            default:
                return string.Empty;
        }
    }

    private static string RenderList(List list, int indentLevel)
    {
        var sb = new StringBuilder();
        foreach (var item in list.Items)
        {
            var indent = new string(' ', indentLevel * 2);
            sb.Append($"[dim]{EscapeMarkup(indent)}* [/]");
            sb.Append(string.Concat(item.Inlines.Select(RenderInline)));
            if (item.NestedList != null)
            {
                sb.Append(RenderList(item.NestedList, indentLevel + 1));
            }
        }
        return sb.ToString();
    }

    private static string EscapeMarkup(string text)
    {
        return text.Replace("[", "[[").Replace("]", "]]");
    }
}


internal static class HtmlToMarkdown
{
    public static string Transform(string html, Core.Status status)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        var doc = AdventOfCodeDocument.LoadFrom(htmlDoc, status);
        return RenderDocument(doc);
    }

    private static string RenderDocument(AdventOfCodeDocument doc)
    {
        var sb = new StringBuilder();
        foreach (var child in doc.Children)
        {
            sb.Append(RenderElement(child));
        }
        if (sb.Length == 0)
            sb.Append("*No content*");
        return sb.ToString();
    }

    private static string RenderElement(DocumentElement elem)
    {
        switch (elem)
        {
            case Heading h:
                var hashes = new string('#', h.Level);
                return $"{hashes} {EscapeMarkdown(h.Text)}\n\n";
            case Paragraph p:
                var markup = string.Concat(p.Inlines.Select(RenderInline));
                return markup + "\n\n";
            case List l:
                return RenderList(l, 0);
            case Article a:
                return string.Concat(a.Children.Select(RenderElement));
            case InlineContent c:
                return string.Concat(c.Inlines.Select(RenderInline)) + "\n\n";
            default:
                return string.Empty;
        }
    }

    private static string RenderInline(InlineElement elem)
    {
        switch (elem)
        {
            case Text t:
                return EscapeMarkdown(t.Content);
            case Bold b:
                return $"**{EscapeMarkdown(b.Content)}**";
            case Italic i:
                return $"*{EscapeMarkdown(i.Content)}*";
            case Code c:
                var inner = string.Concat(c.Inlines.Select(RenderInline));
                return $"`{inner}`";
            case Link l:
                var linkInner = string.Concat(l.Inlines.Select(RenderInline));
                if (!string.IsNullOrEmpty(l.Href))
                {
                    return $"[{linkInner}]({l.Href})";
                }
                return linkInner;
            case EasterEgg e:
                return $"{EscapeMarkdown(e.VisibleText)} ({EscapeMarkdown(e.Tooltip)})";
            default:
                return string.Empty;
        }
    }

    private static string RenderList(List list, int indentLevel)
    {
        var sb = new StringBuilder();
        foreach (var item in list.Items)
        {
            var indent = new string(' ', indentLevel * 2);
            sb.Append($"{indent}- ");
            sb.Append(string.Concat(item.Inlines.Select(RenderInline)));
            sb.Append('\n');
            if (item.NestedList != null)
            {
                sb.Append(RenderList(item.NestedList, indentLevel + 1));
            }
        }
        sb.Append('\n');
        return sb.ToString();
    }

    private static string EscapeMarkdown(string text)
    {
        // Escape markdown special chars: * _ ` [ ] ( ) 
        return text.Replace("\\", "\\\\")
                   .Replace("*", "\\*")
                   .Replace("_", "\\_")
                   .Replace("`", "\\`")
                   .Replace("[", "\\[")
                   .Replace("]", "\\]")
                   .Replace("(", "\\(")
                   .Replace(")", "\\)");
    }
}

































































































































































































































































