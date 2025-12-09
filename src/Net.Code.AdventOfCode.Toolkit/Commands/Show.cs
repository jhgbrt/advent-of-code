namespace Net.Code.AdventOfCode.Toolkit.Commands;

using Net.Code.AdventOfCode.Toolkit.Core;
using Net.Code.AdventOfCode.Toolkit.Infrastructure;
using DocumentParagraph = Core.Paragraph;
using DocumentText = Core.Text;
using System.Text;
using Spectre.Console;
using Spectre.Console.Cli;
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
        var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);
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

    private static string RenderElement(DocumentElement elem) => elem switch
    {
        Heading h => $"\n[bold]{EscapeMarkup(h.Text)}[/]\n\n",
        DocumentParagraph p => string.Concat(p.Inlines.Select(RenderInline)) + "\n",
        List l => RenderList(l, 0),
        Article a => string.Concat(a.Children.Select(RenderElement)),
        InlineContent c => string.Concat(c.Inlines.Select(RenderInline)),
        Preformatted pre => $"\n[dim]{EscapeMarkup(pre.Content)}[/]\n\n",
        _ => string.Empty,
    };

    private static string RenderInline(InlineElement elem)
    {
        return elem switch
        {
            DocumentText t => $"[dim]{EscapeMarkup(t.Content)}[/]",
            Bold b => $"[bold]{EscapeMarkup(b.Content)}[/]",
            Italic i => $"[{(i.IsStarStyled ? "yellow" : "bold")}]{EscapeMarkup(i.Content)}[/]",
            Code c => string.Concat(c.Inlines.Select(RenderInline)),
            Link l => string.Concat(l.Inlines.Select(RenderInline))
                        + (!string.IsNullOrEmpty(l.Href) ? $"[dim] ([/][blue]{EscapeMarkup(l.Href)}[/][dim])[/]" : string.Empty),
            EasterEgg e => $"[dim]{EscapeMarkup(e.VisibleText)}[/][dim] ({EscapeMarkup(e.Tooltip)})[/]",
            _ => string.Empty,
        };
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

    private static string EscapeMarkup(string text) => text.Replace("[", "[[").Replace("]", "]]");
}


internal static class HtmlToMarkdown
{
    public static string Transform(string html, Core.Status status)
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);
        var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);
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
        return elem switch
        {
            Heading h => $"{new string('#', h.Level)} {EscapeMarkdown(h.Text)}\n\n",
            DocumentParagraph p => string.Concat(p.Inlines.Select(RenderInline)) + "\n\n",
            List l => RenderList(l, 0),
            Article a => string.Concat(a.Children.Select(RenderElement)),
            InlineContent c => string.Concat(c.Inlines.Select(RenderInline)) + "\n\n",
            Preformatted pre => $"```\n{pre.Content}\n```\n\n",
            _ => string.Empty,
        };
    }

    private static string RenderInline(InlineElement elem)
    {
        switch (elem)
        {
            case DocumentText t:
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

































































































































































































































































