namespace Net.Code.AdventOfCode.Toolkit.Core;

using HtmlAgilityPack;

using System;
using System.Collections.Generic;
using System.Linq;

internal readonly record struct PuzzleMetadata(Answer Answer, Status Status);

public abstract class DocumentElement { }

public abstract class InlineElement { }

public class Text : InlineElement
{
    public string Content { get; set; } = string.Empty;
}

public class Bold : InlineElement
{
    public string Content { get; set; } = string.Empty;
}

public class Italic : InlineElement
{
    public string Content { get; set; } = string.Empty;
    public bool IsStarStyled { get; set; } // For star-themed italics
}

public class Code : InlineElement
{
    public List<InlineElement> Inlines { get; set; } = new();
}

public class Link : InlineElement
{
    public string Href { get; set; } = string.Empty;
    public List<InlineElement> Inlines { get; set; } = new();
}

public class EasterEgg : InlineElement
{
    public string VisibleText { get; set; } = string.Empty;
    public string Tooltip { get; set; } = string.Empty; // From <span title="...">
}

public class Heading : DocumentElement
{
    public int Level { get; set; } // 1-3 for h1-h3
    public string Text { get; set; } = string.Empty;
}

public class Paragraph : DocumentElement
{
    public List<InlineElement> Inlines { get; set; } = new();
}

public class Preformatted : DocumentElement
{
    public string Content { get; set; } = string.Empty;
}

public class List : DocumentElement
{
    public List<ListItem> Items { get; set; } = new();
}

public class ListItem : DocumentElement
{
    public List<InlineElement> Inlines { get; set; } = new();
    public List? NestedList { get; set; }
}

public class Article : DocumentElement
{
    public List<DocumentElement> Children { get; set; } = new();
}

public class InlineContent : DocumentElement
{
    public List<InlineElement> Inlines { get; set; } = new();
}

public class AdventOfCodeDocument : DocumentElement
{
    public List<DocumentElement> Children { get; set; } = new();
    public string Example { get; private set; } = string.Empty;
    internal PuzzleMetadata Metadata { get; private set; } = new PuzzleMetadata(Answer.Empty, Status.Locked);

    internal static PuzzleMetadata ExtractPuzzleMetadata(HtmlDocument htmlDoc)
    {
        if (htmlDoc == null)
            throw new ArgumentNullException(nameof(htmlDoc));

        var answer = ExtractAnswers(htmlDoc);
        var status = DetermineStatus(htmlDoc, answer);
        return new PuzzleMetadata(answer, status);
    }

    public static AdventOfCodeDocument LoadFrom(HtmlDocument htmlDoc)
    {
        if (htmlDoc == null)
            throw new ArgumentNullException(nameof(htmlDoc));

        var metadata = ExtractPuzzleMetadata(htmlDoc);
        var main = htmlDoc.DocumentNode.SelectSingleNode("//main");

        AdventOfCodeDocument document;
        if (main == null)
        {
            document = new AdventOfCodeDocument();
        }
        else
        {
            document = BuildDocument(main, metadata);
            document.Example = ExtractExample(main).Replace("\r\n", "\n").Replace("\r", "\n");
        }

        document.Metadata = metadata;
        return document;
    }

    private static AdventOfCodeDocument BuildDocument(HtmlNode main, PuzzleMetadata metadata)
    {
        var doc = new AdventOfCodeDocument();
        var state = metadata.Status;

        foreach (var node in main.ChildNodes)
        {
            string nodeName = node.Name.ToLowerInvariant();
            string nodeClass = node.GetAttributeValue("class", string.Empty);

            if (state == Core.Status.Completed)
            {
                if (nodeName == "p" && nodeClass.Contains("day-success", StringComparison.OrdinalIgnoreCase))
                    break;
            }
            else if (state == Core.Status.Unlocked)
            {
                if (nodeName == "article")
                {
                    doc.Children.Add(BuildBlockElement(node));
                    break;
                }
                continue;
            }
            else if (state == Core.Status.AnsweredPart1)
            {
                if (nodeName is "form")
                    break;
            }

            if (nodeName is "article" or "p" or "h1" or "h2" or "h3")
            {
                doc.Children.Add(BuildBlockElement(node));
            }
            else if (nodeName is "ul")
            {
                doc.Children.Add(BuildListElement(node, 0));
            }

            if (state == Core.Status.AnsweredPart1)
            {
                if (nodeName is "p" && nodeClass.Contains("day-success", StringComparison.OrdinalIgnoreCase))
                {
                    var next = node.NextSibling;
                    while (next != null && next.NodeType != HtmlNodeType.Element)
                        next = next.NextSibling;
                    if (next != null && next.Name.Equals("article", StringComparison.OrdinalIgnoreCase))
                        doc.Children.Add(BuildBlockElement(next));
                    break;
                }
            }
        }

        return doc;
    }

    private static string ExtractExample(HtmlNode main)
    {
        var paragraph = main
            .Descendants("p")
            .FirstOrDefault(p => string.Equals(p.InnerText?.Trim(), "For example:", StringComparison.OrdinalIgnoreCase));

        if (paragraph == null)
            return string.Empty;

        var next = paragraph.NextSibling;
        while (next != null && next.NodeType != HtmlNodeType.Element)
            next = next.NextSibling;

        if (next == null || !next.Name.Equals("pre", StringComparison.OrdinalIgnoreCase))
            return string.Empty;

        var codeNode = next.SelectSingleNode("./code") ?? next;
        var content = HtmlEntity.DeEntitize(codeNode.InnerText ?? string.Empty);
        return content;
    }

    private static DocumentElement BuildBlockElement(HtmlNode node)
    {
        var name = node.Name.ToLowerInvariant();
        return name switch
        {
            "h1" or "h2" or "h3" => new Heading { Level = int.Parse(name[1].ToString()), Text = node.InnerText?.Trim() ?? string.Empty },
            "p" => new Paragraph { Inlines = BuildInlineElements(node) },
            "pre" => new Preformatted { Content = HtmlEntity.DeEntitize(node.InnerText ?? string.Empty) },
            "article" => new Article { Children = BuildChildren(node) },
            "ul" => BuildListElement(node, 0),
            _ => new Article { Children = BuildChildren(node) }
        };
    }

    private static List<DocumentElement> BuildChildren(HtmlNode node)
    {
        var children = new List<DocumentElement>();
        bool hasBlock = false;
        foreach (var child in node.ChildNodes)
        {
            if (child.NodeType == HtmlNodeType.Element)
            {
                var name = child.Name.ToLowerInvariant();
                if (name is "article" or "p" or "pre" or "h1" or "h2" or "h3" or "ul")
                {
                    hasBlock = true;
                    children.Add(BuildBlockElement(child));
                }
            }
        }
        if (!hasBlock)
        {
            children.Add(new InlineContent { Inlines = BuildInlineElements(node) });
        }
        return children;
    }

    private static List<InlineElement> BuildInlineElements(HtmlNode node)
    {
        var list = new List<InlineElement>();
        foreach (var child in node.ChildNodes)
        {
            list.AddRange(ConvertNodeToInlineElement(child));
        }
        if (list.Count == 0)
        {
            list.Add(new Text { Content = node.InnerText ?? string.Empty });
        }
        return list;
    }

    private static IEnumerable<InlineElement> ConvertNodeToInlineElement(HtmlNode node)
    {
        switch (node.NodeType)
        {
            case HtmlNodeType.Text:
                yield return new Text { Content = node.InnerText ?? string.Empty };
                break;
            case HtmlNodeType.Element:
                var name = node.Name.ToLowerInvariant();
                var elements = name switch
                {
                    "b" or "strong" => [new Bold { Content = node.InnerText ?? string.Empty }],
                    "em" or "i" => [new Italic { Content = node.InnerText ?? string.Empty, IsStarStyled = node.GetAttributeValue("class", string.Empty).Contains("star", StringComparison.OrdinalIgnoreCase) }],
                    "code" => [new Code { Inlines = BuildInlineElements(node) }],
                    "br" => [new Text { Content = "\n" }],
                    "a" => [new Link { Href = node.GetAttributeValue("href", string.Empty), Inlines = BuildInlineElements(node) }],
                    "span" => HandleSpan(node),
                    "ul" => [],
                    "li" => [],
                    "p" or "pre" => BuildInlineElements(node).Append(new Text { Content = "\n" }),
                    "h1" or "h2" or "h3" => [new Text { Content = "\n" }, new Bold { Content = node.InnerText?.Trim() ?? string.Empty }, new Text { Content = "\n\n" }],
                    _ => BuildInlineElements(node)
                };
                foreach (var elem in elements)
                    yield return elem;
                break;
        }
    }

    private static IEnumerable<InlineElement> HandleSpan(HtmlNode node)
    {
        var title = node.GetAttributeValue("title", string.Empty);
        if (!string.IsNullOrEmpty(title))
        {
            yield return new EasterEgg { VisibleText = node.InnerText ?? string.Empty, Tooltip = title };
        }
        else
        {
            foreach (var elem in BuildInlineElements(node))
                yield return elem;
        }
    }

    private static List BuildListElement(HtmlNode ulNode, int indentLevel)
    {
        var list = new List();
        foreach (var li in ulNode.SelectNodes("./li") ?? Enumerable.Empty<HtmlNode>())
        {
            var item = new ListItem { Inlines = BuildInlineElements(li) };
            foreach (var childUl in li.SelectNodes("./ul") ?? Enumerable.Empty<HtmlNode>())
            {
                item.NestedList = BuildListElement(childUl, indentLevel + 1);
            }
            list.Items.Add(item);
        }
        return list;
    }

    private static Answer ExtractAnswers(HtmlDocument htmlDoc)
    {
        var answers = (
            from node in htmlDoc.DocumentNode.Descendants("p")
            let text = node.InnerText?.TrimStart() ?? string.Empty
            where text.StartsWith("Your puzzle answer was", StringComparison.OrdinalIgnoreCase)
            let code = node.SelectSingleNode(".//code")?.InnerText ?? string.Empty
            where !string.IsNullOrEmpty(code)
            select code
        ).ToArray();

        return answers.Length switch
        {
            0 => Answer.Empty,
            1 => new Answer(answers[0], string.Empty),
            2 => new Answer(answers[0], answers[1]),
            _ => throw new Exception($"expected 0, 1 or 2 answers, not {answers.Length}")
        };
    }

    private static Status DetermineStatus(HtmlDocument htmlDoc, Answer answer)
    {
        if (!string.IsNullOrEmpty(answer.part2) || HasDaySuccessParagraph(htmlDoc, "Both parts"))
            return Status.Completed;

        if (HasSubmissionForm(htmlDoc, "2"))
            return Status.AnsweredPart1;

        if (!string.IsNullOrEmpty(answer.part1))
            return Status.AnsweredPart1;

        if (HasSubmissionForm(htmlDoc, "1"))
            return Status.Unlocked;

        return Status.Unlocked;
    }

    private static bool HasSubmissionForm(HtmlDocument htmlDoc, string level)
    {
        return htmlDoc.DocumentNode
            .Descendants("form")
            .Any(form => form
                .Descendants("input")
                .Any(input => input.GetAttributeValue("name", string.Empty) == "level"
                           && input.GetAttributeValue("value", string.Empty) == level));
    }

    private static bool HasDaySuccessParagraph(HtmlDocument htmlDoc, string containsText)
    {
        return htmlDoc.DocumentNode
            .Descendants("p")
            .Any(p =>
                p.GetAttributeValue("class", string.Empty)
                 .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                 .Any(className => className.Equals("day-success", StringComparison.OrdinalIgnoreCase))
                && (containsText == null
                    || p.InnerText.Contains(containsText, StringComparison.OrdinalIgnoreCase)));
    }
}