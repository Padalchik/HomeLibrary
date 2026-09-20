namespace HomeLibrary.Web.Models.Books;

public sealed class BookDetailsViewModel
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string Author { get; init; } = string.Empty;

    public int PublicationYear { get; init; }

    public string TableOfContents { get; init; } = string.Empty;

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}
