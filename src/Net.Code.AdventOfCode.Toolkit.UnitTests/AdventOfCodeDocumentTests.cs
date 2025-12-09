using HtmlAgilityPack;

using Net.Code.AdventOfCode.Toolkit.Core;

namespace Net.Code.AdventOfCode.Toolkit.UnitTests
{
    public class AdventOfCodeDocumentTests
    {
        [Fact]
        public void LoadFrom_UnlockedPuzzle_CreatesDocument()
        {
            var html = @"<html><body><main><article><h2>--- Day 9: Sensor Boost ---</h2><p>Description here.</p></article></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);

            Assert.NotNull(doc);
            Assert.Single(doc.Children);
            var article = Assert.IsType<Article>(doc.Children[0]);
            Assert.Equal(2, article.Children.Count);
            Assert.IsType<Heading>(article.Children[0]);
            Assert.IsType<Paragraph>(article.Children[1]);
        }

        [Fact]
        public void LoadFrom_AnsweredPart1_CreatesDocument()
        {
            var html = @"<html><body><main><article><h2>--- Day 1: Title ---</h2><p>Part 1</p></article><p>Your puzzle answer was <code>123</code>.</p><p class='day-success'>Part 1 complete</p><article><h2>--- Part Two ---</h2><p>Part 2</p></article><form>Answer form</form></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);

            Assert.NotNull(doc);
            Assert.True(doc.Children.Count >= 2); // Original article and next
            var headings = doc.Children.OfType<Article>().SelectMany(a => a.Children).OfType<Heading>();
            Assert.True(headings.Count() >= 2);
        }

        [Fact]
        public void LoadFrom_CompletedPuzzle_CreatesDocument()
        {
            var html = @"<html><body><main><article><h2>--- Day 1: Title ---</h2><p>Part 1</p></article><p>Your puzzle answer was <code>123</code>.</p><p class='day-success'>Part 1 complete</p><article><h2>--- Part Two ---</h2><p>Part 2</p></article><p>Your puzzle answer was <code>456</code>.</p><p class='day-success'>Complete</p><p>Extra</p></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);

            Assert.NotNull(doc);
            // Should stop at the success message
            Assert.DoesNotContain(doc.Children, c => c is Paragraph p && p.Inlines.Any(i => i is Text t && t.Content.Contains("Extra")));
        }

        [Fact]
        public void LoadFrom_NoMain_ReturnsEmptyDocument()
        {
            var html = @"<html><body></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);

            Assert.NotNull(doc);
            Assert.Empty(doc.Children);
        }

        [Fact]
        public void LoadFrom_IncludesEasterEggs()
        {
            var html = @"<html><body><main><article><p>Good luck! <span title=""Easter egg"">text</span></p></article></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);

            var article = Assert.IsType<Article>(doc.Children[0]);
            var paragraph = Assert.IsType<Paragraph>(article.Children[0]);
            var easterEgg = paragraph.Inlines.OfType<EasterEgg>().FirstOrDefault();
            Assert.NotNull(easterEgg);
            Assert.Equal("text", easterEgg.VisibleText);
            Assert.Equal("Easter egg", easterEgg.Tooltip);
        }

        [Fact]
        public void LoadFrom_IncludesListsAndNestedElements()
        {
            var html = @"<html><body><main><article><h2>Title</h2><p>Text with <code>code</code> and <a href='link'>link</a>.</p><ul><li>Item 1</li><li>Item 2<ul><li>Subitem</li></ul></li></ul></article></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);

            Assert.NotNull(doc);
            var article = Assert.IsType<Article>(doc.Children[0]);
            Assert.Equal(3, article.Children.Count); // Heading, Paragraph, List
            var list = Assert.IsType<List>(article.Children[2]);
            Assert.Equal(2, list.Items.Count);
            var subList = list.Items[1].NestedList;
            Assert.NotNull(subList);
            Assert.Single(subList.Items);
        }

        [Fact]
        public void LoadFrom_ExtractsExample()
        {
            var html = """
                <html><body><main><article>
                    <p>Intro</p>
                    <p>For example:</p>
                    <pre>
                        <code>7,1
                11,1
                </code>
                </pre>
                </article>
                </main></body></html>
                """;
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc);

            Assert.Equal("7,1\n11,1\n", doc.Example);
        }

        [Fact]
        public void ExtractPuzzleMetadata_UnlockedPuzzle()
        {
            var html = @"<html><body><main><article class='day-desc'><p>Part 1 description</p></article><form method='post' action='1/answer'><input type='hidden' name='level' value='1'/></form></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var metadata = AdventOfCodeDocument.ExtractPuzzleMetadata(htmlDoc);

            Assert.Equal(Status.Unlocked, metadata.Status);
            Assert.Equal(string.Empty, metadata.Answer.part1);
            Assert.Equal(string.Empty, metadata.Answer.part2);
        }

        [Fact]
        public void ExtractPuzzleMetadata_AnsweredPart1()
        {
            var html = @"<html><body><main><p>Your puzzle answer was <code>123</code>.</p><p class='day-success'>Part 1 complete</p><form method='post' action='1/answer'><input type='hidden' name='level' value='2'/></form></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var metadata = AdventOfCodeDocument.ExtractPuzzleMetadata(htmlDoc);

            Assert.Equal(Status.AnsweredPart1, metadata.Status);
            Assert.Equal("123", metadata.Answer.part1);
            Assert.Equal(string.Empty, metadata.Answer.part2);
        }

        [Fact]
        public void ExtractPuzzleMetadata_Completed()
        {
            var html = @"<html><body><main><p>Your puzzle answer was <code>123</code>.</p><p>Your puzzle answer was <code>456</code>.</p><p class='day-success'>Both parts of this puzzle are complete!</p></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var metadata = AdventOfCodeDocument.ExtractPuzzleMetadata(htmlDoc);

            Assert.Equal(Status.Completed, metadata.Status);
            Assert.Equal("123", metadata.Answer.part1);
            Assert.Equal("456", metadata.Answer.part2);
        }
    }
}