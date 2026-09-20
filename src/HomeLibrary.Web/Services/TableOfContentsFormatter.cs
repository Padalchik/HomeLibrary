using System.Xml;
using System.Xml.Linq;
using AngleSharp.Html.Parser;
using Ganss.Xss;

namespace HomeLibrary.Web.Services;

public sealed class TableOfContentsFormatter : ITableOfContentsFormatter
{
    private const string RootElementName = "TableOfContents";

    private static readonly string[] AllowedTags =
    [
        "p", "br", "strong", "b", "em", "i", "u",
        "ul", "ol", "li", "h1", "h2", "h3", "h4"
    ];

    private readonly IHtmlSanitizer sanitizer;

    public TableOfContentsFormatter()
    {
        var options = new HtmlSanitizerOptions();
        options.AllowedTags.Clear();
        options.AllowedTags.UnionWith(AllowedTags);
        options.AllowedAttributes.Clear();
        sanitizer = new HtmlSanitizer(options);
    }

    public string ToXml(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        var sanitizedHtml = sanitizer.Sanitize(html);
        var document = new HtmlParser().ParseDocument(sanitizedHtml);
        if (string.IsNullOrWhiteSpace(document.Body?.TextContent))
        {
            throw new ArgumentException("Оглавление не может быть пустым.", nameof(html));
        }

        return new XElement(RootElementName, sanitizedHtml).ToString(SaveOptions.DisableFormatting);
    }

    public string FromXml(string xml)
    {
        ArgumentNullException.ThrowIfNull(xml);

        var root = XElement.Parse(xml, LoadOptions.PreserveWhitespace);
        if (root.Name != RootElementName)
        {
            throw new XmlException(
                $"Expected root element '{RootElementName}', but found '{root.Name}'.");
        }

        return sanitizer.Sanitize(root.Value);
    }
}
