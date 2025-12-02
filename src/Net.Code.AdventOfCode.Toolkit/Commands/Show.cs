namespace Net.Code.AdventOfCode.Toolkit.Commands;

using Net.Code.AdventOfCode.Toolkit.Core;
using Net.Code.AdventOfCode.Toolkit.Infrastructure;

using Spectre.Console.Rendering;
using Spectre.Console;

using System;
using System.ComponentModel;
using System.Threading;
using HtmlAgilityPack;

[Description("Render the instructions.")]
class Show(IPuzzleManager puzzleManager, AoCLogic aocLogic, IInputOutputService io) : ManyPuzzlesCommand<AoCSettings>(aocLogic)
{
    public override async Task<int> ExecuteAsync(PuzzleKey key, AoCSettings _, CancellationToken ct)
    {
        var puzzle = await puzzleManager.SyncPuzzle(key);

        io.Write(new PuzzleHtmlRenderer(puzzle));
        return 0;
    }
    class PuzzleHtmlRenderer(Puzzle puzzle) : Renderable
    {
        // A Puzzle html document for a completed puzzle is structured as follows:
        // <html>
        //  <head>...</head>
        //  <body>
        //    <header>...</header> 
        //    <div id="sidebar">...</div>
        //    <main>
        //      <article class="day-desc">...</article>                 <-- description of part 1
        //      <p>...</p>                                              <-- answer for part 1
        //      <article class="day-desc">...</article>                 <-- description of part 2
        //      <p>...</p>                                              <-- answer for part 2
        //      <p class="day-success">...</p>                          <-- success message after submitting both answers
        //      <p>...</p>                                              <-- other paragraphs, to be ignored
        //    </main>
        //  </body>
        // </html>

        // A Puzzle html document when unauthorized looks like this:
        // <html>
        //  <head>...</head>
        //  <body>
        //    <header>...</header> 
        //    <div id="sidebar">...</div>
        //    <main>
        //      <article class="day-desc">...</article>                             <-- description of part 1
        //      <p>To play, please identify yourself via one of these services:</p> <-- authorization message, to be ignored with the rest of the document
        //    </main>
        //  </body>
        // </html>

        // A Puzzle html document when authorized, but no puzzle completed looks like this:
        // <html>
        //  <head>...</head>
        //  <body>
        //    <header>...</header> 
        //    <div id="sidebar">...</div>
        //    <main>
        //      <article class="day-desc">...</article>                                       <-- description of part 1
        //      <p>To begin, <a href="1/input" target="_blank">get your puzzle input</a>.</p> <-- get input message, to be ignored with the rest of the document
        //    </main>
        //  </body>
        // </html>

        // A Puzzle html document when authorized, with part 1 completed looks like this:
        // <html>
        //  <head>...</head>
        //  <body>
        //    <header>...</header> 
        //    <div id="sidebar">...</div>
        //    <main>
        //      <article class="day-desc">...</article>                                       <-- description of part 1
        //      <p>Your puzzle answer was <code>[ANSWER HERE]</code>.</p>
        //      <p class="day-success">The first half of this puzzle is complete! It provides one gold star: *</p>
        //      <article class="day-desc">...</article>                                       <-- description of part 2
        //      <form method="post" action="1/answer"><input type="hidden" name="level" value="2"><p>Answer: <input type="text" name="answer" autocomplete="off"> <input type="submit" value="[Submit]"></p></form>  <-- answer form for part 2 - to be ignored with the rest of the document
        //    </main>
        //  </body>
        // </html>


        protected override IEnumerable<Segment> Render(RenderOptions options, int maxWidth)
        {
            var document = new HtmlDocument();
            document.LoadHtml(puzzle.Html);
            var main = document.DocumentNode.SelectSingleNode("//main");
            if (main == null)
                return [new Segment("No content", Style.Plain)];

            var segments = new List<Segment>();


            var state = puzzle.Status;

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
                        segments.AddRange(BuildBlockSegments(node));
                        break;
                    }
                    continue;
                }
                else if (state == Core.Status.AnsweredPart1)
                {
                    if (nodeName == "form")
                        break;
                }

                if (nodeName is "article" or "p" or "h1" or "h2" or "h3")
                {
                    segments.AddRange(BuildBlockSegments(node));
                }
                else if (nodeName is "ul")
                {
                    segments.AddRange(BuildListSegments(node, 0));
                    segments.Add(Segment.LineBreak);
                }

                if (state == Core.Status.AnsweredPart1)
                {
                    if (nodeName is "p" && nodeClass.Contains("day-success", StringComparison.OrdinalIgnoreCase))
                    {
                        var next = node.NextSibling;
                        while (next != null && next.NodeType != HtmlNodeType.Element)
                            next = next.NextSibling;
                        if (next != null && next.Name.Equals("article", StringComparison.OrdinalIgnoreCase))
                            segments.AddRange(BuildBlockSegments(next));
                        break;
                    }
                }
            }

            if (segments.Count == 0)
                segments.Add(new Segment("No renderable content", Style.Plain));

            return segments;
        }

        private static IEnumerable<Segment> BuildBlockSegments(HtmlNode node)
        {
            var name = node.Name.ToLowerInvariant();
            if (name is "h1" or "h2" or "h3")
            {
                var text = node.InnerText?.Trim() ?? string.Empty;
                return [new Segment(text, new Style(null, null, Decoration.Bold)), Segment.LineBreak, Segment.LineBreak];
            }
            if (name is "p")
            {
                var segs = BuildInlineSegments(node).ToList();
                return segs;
            }
            return BuildInlineSegments(node);
        }

        private static IEnumerable<Segment> BuildInlineSegments(HtmlNode node)
        {
            var list = new List<Segment>();
            foreach (var child in node.ChildNodes)
            {
                list.AddRange(ConvertNodeToInlineSegments(child));
            }
            if (list.Count == 0)
            {
                var text = node.InnerText ?? string.Empty;
                list.Add(new Segment(text, Style.Plain));
            }
            return list;
        }

        private static IEnumerable<Segment> ConvertNodeToInlineSegments(HtmlNode node)
        {
            switch (node.NodeType)
            {
                case HtmlNodeType.Text:
                return new[] { new Segment(node.InnerText ?? string.Empty, Style.Plain) };
                case HtmlNodeType.Element:
                var name = node.Name.ToLowerInvariant();
                if (name == "b" || name == "strong")
                {
                    return BuildStyledInline(node, new Style(null, null, Decoration.Bold));
                }
                if (name == "em" || name == "i")
                {
                    return BuildStyledInline(node, new Style(null, null, Decoration.Italic));
                }
                if (name == "code")
                {
                    // Render code as grey, preserve inner text
                    var text = node.InnerText ?? string.Empty;
                    return [new Segment(text, new Style(foreground: Color.Grey)),];
                }
                if (name == "br")
                {
                    return [Segment.LineBreak];
                }
                if (name == "a")
                {
                    var href = node.GetAttributeValue("href", string.Empty);
                    var inner = BuildInlineSegments(node).ToList();
                    if (!string.IsNullOrEmpty(href))
                    {
                        inner.Add(new Segment(" (", Style.Plain));
                        inner.Add(new Segment(href, new Style(foreground: Color.Blue)));
                        inner.Add(new Segment(")", Style.Plain));
                    }
                    return inner;
                }
                if (name == "ul")
                {
                    return BuildListSegments(node, 0);
                }
                if (name == "li")
                {
                    var segs = new List<Segment>();
                    segs.Add(new Segment("* ", Style.Plain));
                    segs.AddRange(BuildInlineSegments(node));
                    segs.Add(Segment.LineBreak);
                    return segs;
                }
                if (name is "p" or "pre")
                {
                    var segs = BuildInlineSegments(node).ToList();
                    segs.Add(Segment.LineBreak);
                    return segs;
                }
                if (name == "h1" || name == "h2" || name == "h3")
                {
                    var text = node.InnerText?.Trim() ?? string.Empty;
                    return
                    [
                        new Segment(text, new Style(null, null, Decoration.Bold)),
                        Segment.LineBreak,
                        Segment.LineBreak
                    ];
                }
                // Default: recurse
                return BuildInlineSegments(node);
                default:
                return Enumerable.Empty<Segment>();
            }
        }

        private static IEnumerable<Segment> BuildStyledInline(HtmlNode node, Style style)
        {
            var text = node.InnerText ?? string.Empty;
            return new[] { new Segment(text, style) };
        }

        private static IEnumerable<Segment> BuildListSegments(HtmlNode ulNode, int indentLevel)
        {
            var segs = new List<Segment>();
            foreach (var li in ulNode.SelectNodes("./li") ?? Enumerable.Empty<HtmlNode>())
            {
                var indent = new string(' ', indentLevel * 2);
                segs.Add(new Segment(indent, Style.Plain));
                segs.Add(new Segment("* ", Style.Plain));
                segs.AddRange(BuildInlineSegments(li));
                segs.Add(Segment.LineBreak);
                foreach (var childUl in li.SelectNodes("./ul") ?? Enumerable.Empty<HtmlNode>())
                {
                    segs.AddRange(BuildListSegments(childUl, indentLevel + 1));
                }
            }
            return segs;
        }
    }
}


