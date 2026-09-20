namespace HomeLibrary.Web.Services;

public interface ITableOfContentsFormatter
{
    string ToXml(string html);

    string FromXml(string xml);
}
