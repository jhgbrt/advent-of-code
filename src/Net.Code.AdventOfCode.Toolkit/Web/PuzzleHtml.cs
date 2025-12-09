using Net.Code.AdventOfCode.Toolkit.Core;

namespace Net.Code.AdventOfCode.Toolkit.Web;

using HtmlAgilityPack;

record PuzzleHtml(PuzzleKey key, string html, string input)
{
    public Puzzle GetPuzzle()
    {
        var document = new HtmlDocument();
        document.LoadHtml(html);
        var aocdoc = AdventOfCodeDocument.LoadFrom(document);
        var metadata = aocdoc.Metadata;

        return new Puzzle(key, input, aocdoc.Example, metadata.Answer, metadata.Status, html);
    }
}
