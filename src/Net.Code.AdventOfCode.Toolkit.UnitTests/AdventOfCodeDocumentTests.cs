using Net.Code.AdventOfCode.Toolkit.Commands;

using HtmlAgilityPack;

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
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc, Core.Status.Unlocked);

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
            var html = @"<html><body><main><article><h2>--- Day 1: Title ---</h2><p>Part 1</p></article><p>Your answer was 123.</p><p class='day-success'>Part 1 complete</p><article><h2>--- Part Two ---</h2><p>Part 2</p></article><form>Answer form</form></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc, Core.Status.AnsweredPart1);

            Assert.NotNull(doc);
            Assert.True(doc.Children.Count >= 2); // Original article and next
            var headings = doc.Children.OfType<Article>().SelectMany(a => a.Children).OfType<Heading>();
            Assert.True(headings.Count() >= 2);
        }

        [Fact]
        public void LoadFrom_CompletedPuzzle_CreatesDocument()
        {
            var html = @"<html><body><main><article><h2>--- Day 1: Title ---</h2><p>Part 1</p></article><p>Your answer was 123.</p><p class='day-success'>Part 1 complete</p><article><h2>--- Part Two ---</h2><p>Part 2</p></article><p>Your answer was 456.</p><p class='day-success'>Complete</p><p>Extra</p></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc, Core.Status.Completed);

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
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc, Core.Status.Unlocked);

            Assert.NotNull(doc);
            Assert.Empty(doc.Children);
        }

        [Fact]
        public void LoadFrom_IncludesEasterEggs()
        {
            var html = @"<html><body><main><article><p>Good luck! <span title=""Easter egg"">text</span></p></article></main></body></html>";
            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc, Core.Status.Unlocked);

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
            var doc = AdventOfCodeDocument.LoadFrom(htmlDoc, Core.Status.Unlocked);

            Assert.NotNull(doc);
            var article = Assert.IsType<Article>(doc.Children[0]);
            Assert.Equal(3, article.Children.Count); // Heading, Paragraph, List
            var list = Assert.IsType<List>(article.Children[2]);
            Assert.Equal(2, list.Items.Count);
            var subList = list.Items[1].NestedList;
            Assert.NotNull(subList);
            Assert.Single(subList.Items);
        }
    }
}