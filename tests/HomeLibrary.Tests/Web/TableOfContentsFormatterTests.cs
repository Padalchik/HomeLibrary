using System.Xml.Linq;
using System.Xml;
using HomeLibrary.Web.Services;

namespace HomeLibrary.Tests.Web;

public sealed class TableOfContentsFormatterTests
{
    private readonly TableOfContentsFormatter formatter = new();

    [Fact]
    public void ToXml_WrapsHtmlAsEscapedTextInExpectedRoot()
    {
        const string html = "<p><strong>Chapter 1</strong></p>";

        var xml = formatter.ToXml(html);
        var root = XElement.Parse(xml);

        Assert.Equal("TableOfContents", root.Name.LocalName);
        Assert.Equal(html, root.Value);
        Assert.Empty(root.Elements());
    }

    [Fact]
    public void ToXml_EscapesSpecialCharactersAndProducesValidXml()
    {
        const string html = "<p>Rock &amp; Roll: 2 &lt; 3 &gt; 1</p>";

        var xml = formatter.ToXml(html);
        var root = XElement.Parse(xml);

        Assert.Equal(html, root.Value);
    }

    [Fact]
    public void FromXml_ReturnsAllowedHtml()
    {
        const string html = "<p><strong>Chapter</strong> <em>one</em></p><ul><li>Topic</li></ul>";
        var xml = new XElement("TableOfContents", html).ToString(SaveOptions.DisableFormatting);

        var result = formatter.FromXml(xml);

        Assert.Equal(html, result);
    }

    [Fact]
    public void ToXml_RemovesScriptElements()
    {
        var xml = formatter.ToXml("<p>Safe</p><script>alert('xss')</script>");

        var html = XElement.Parse(xml).Value;

        Assert.Equal("<p>Safe</p>", html);
        Assert.DoesNotContain("script", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToXml_RemovesAllAttributesIncludingEventHandlers()
    {
        var xml = formatter.ToXml("<p onclick=\"alert(1)\">Text</p><strong onerror=\"alert(2)\">Bold</strong>");

        var html = XElement.Parse(xml).Value;

        Assert.Equal("<p>Text</p><strong>Bold</strong>", html);
        Assert.DoesNotContain("onerror", html, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("onclick", html, StringComparison.OrdinalIgnoreCase);
    }

    [Theory]
    [InlineData("<img src=\"https://example.com/image.jpg\">", "img")]
    [InlineData("<a href=\"https://example.com\">Link</a>", "a")]
    [InlineData("<table><tr><td>Cell</td></tr></table>", "table")]
    public void ToXml_RemovesTagsOutsideAllowlist(string input, string forbiddenTag)
    {
        var xml = formatter.ToXml(input);

        var html = XElement.Parse(xml).Value;

        Assert.DoesNotContain($"<{forbiddenTag}", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToXml_RemovesStyleAttribute()
    {
        var xml = formatter.ToXml("<p style=\"color:red\">Text</p>");

        var html = XElement.Parse(xml).Value;

        Assert.Equal("<p>Text</p>", html);
        Assert.DoesNotContain("style", html, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void ToXml_PreservesEveryAllowedTag()
    {
        const string input =
            "<h1>H1</h1><h2>H2</h2><h3>H3</h3><h4>H4</h4>" +
            "<p><strong>Strong</strong><em>Emphasis</em></p>" +
            "<ul><li>Bullet</li></ul><ol><li>Numbered</li></ol>";

        var xml = formatter.ToXml(input);

        var html = XElement.Parse(xml).Value;

        Assert.Equal(input, html);
    }

    [Fact]
    public void FromXml_SanitizesStoredHtmlAgain()
    {
        var xml = new XElement(
            "TableOfContents",
            "<p onclick=\"alert(1)\">Safe</p><iframe src=\"https://example.com\"></iframe>")
            .ToString(SaveOptions.DisableFormatting);

        var html = formatter.FromXml(xml);

        Assert.Equal("<p>Safe</p>", html);
    }

    [Fact]
    public void FromXml_ThrowsWhenRootElementIsUnexpected()
    {
        Assert.Throws<XmlException>(() => formatter.FromXml("<Contents>text</Contents>"));
    }
}
